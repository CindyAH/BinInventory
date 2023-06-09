using System.Net;
using System.Web.Http;
using BinInventory.Models;
using Newtonsoft.Json;

namespace BinInventory
{

    /// <summary>
    /// Controller to handle the API requests for bins and items within bins
    /// </summary>
    public class BinInventoryController : ApiController
    {
        BinCollection _binCollection = new BinCollection();

        /// <summary>
        /// Get all the bins
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/bins")]
        public HttpResponseMessage GetBins()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _binCollection.Bins);
        }

        /// <summary>
        /// Get a specific bin by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/bins/{id}")]
        public HttpResponseMessage GetBin(int id)
        {
            var bin = _binCollection.GetById(id);

            if (bin == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, bin);
        }

        /// <summary>
        /// Get all the items in a specific bin by bin id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/bins/{id}/items")]
        public HttpResponseMessage GetItems(int id)
        {
            Bin? bin = _binCollection.GetById(id);

            if (bin == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, bin.Items);
        }

        /// <summary>
        /// Get a specific item by its id in a specific bin by its id
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/bins/{binId}/items/{itemId}")]
        public HttpResponseMessage GetItem(int binId, int itemId)
        {
            // ToDo structure this properly, using business classes and minimal logic in the controller, as is done in AdjustItemCount

            // Find the bin with the matching id in the bins list
            Bin? bin = _binCollection.GetById(binId);

            if (bin == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Bin {binId}");
            }

            // Find the item with the matching id in the items list of the bin
            var item = bin.GetItem(itemId);

            if (item == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Item {itemId}");
            }

            // Return a success response with the item as the content
            return Request.CreateResponse(HttpStatusCode.OK, item);
        }

        /// <summary>
        /// Add a new bin - called by a SmartRack hook?  Called by Kafka 
        /// when SmartRack announces it has one?
        /// </summary>
        /// <param name="bin"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bins")]
        public HttpResponseMessage PostBin([FromBody] Bin bin)
        {
            // ToDo structure this properly, using business classes and minimal logic in the controller, as is done in AdjustItemCount

            // Validate the bin object
            if (bin == null || bin.Id == 0 || string.IsNullOrEmpty(bin.Description) || bin.Items == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            // Check if the bin id already exists in the bins list
            if (_binCollection.Bins.Any(b => b.Id == bin.Id))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            _binCollection.Bins.Add(bin);

            return Request.CreateResponse(HttpStatusCode.Created, bin.Id);
        }

        /// <summary>
        /// AdjustItemCount for an item in a bin
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="itemId"></param>
        /// <param name="newCount"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/bins/{binId}/items/{itemId}/{newCount}")]
        public HttpResponseMessage AdjustItemCount(int binId, int itemId, int newCount)
        {
            try
            {
                Bin? bin = _binCollection.GetById(binId);
                if (bin == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, $"Bin {binId}");
                }
                bin.AdjustItemCount(itemId, newCount);
            }
            catch (ArgumentException aex) 
            { 
                return Request.CreateResponse(HttpStatusCode.BadRequest, aex.Message); 
            }
            catch (KeyNotFoundException kex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Bin {kex.Message}");
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Add a new item to a specific bin by its id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns>Conflict if the item already exists in the bin</returns>
        [HttpPost]
        [Route("api/bins/{id}/items")]
        public HttpResponseMessage PostItem(int id, [FromBody] Item item)
        {
            // ToDo structure this properly, using business classes and minimal logic in the controller, as is done in AdjustItemCount

            // Validate the item object
            if (item == null || item.Id == 0 || string.IsNullOrEmpty(item.Description) || item.Quantity < 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Bin? bin = _binCollection.GetById(id);

            if (bin == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            // Check if the item id already exists in the items list of the bin
            if (bin.Items.Any(i => i.Id == item.Id))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            bin.Items.Add(item);

            return Request.CreateResponse(HttpStatusCode.Created, item.Id);
        }

        /// <summary>
        /// Delete an item (an entire item record including its count, not a single item) from a specific bin
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public HttpResponseMessage DeleteItemFromBin(int binId, int itemId)
        {
            // ToDo structure this properly, with the logic in the Bin class and minimal logic in the controller, as is done in AdjustItemCount

            var bin = _binCollection.GetById(binId);

            if (bin == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            Item item = bin.GetItem(itemId);

            bin.Items.Remove(item);
#warning doesn't actually save

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }}
}
