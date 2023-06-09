using BinInventory.Models;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;

namespace BinInventory.Tests
{
    // A class to test the BinController class
    [TestClass]
    public class BinControllerTest
    {
        private BinInventoryController? _controller;

        [TestInitialize]
        public void SetUp()
        {
            // Create a new instance of the BinController class
            _controller = new BinInventoryController
            {
                // Set up a fake request and configuration for the controller
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [TestMethod]
        public void GetBins_ReturnsAllBins()
        {
            // Act
            var response = _controller.GetBins();
            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(content);

            var bins = JsonConvert.DeserializeObject<List<Bin>>(content);

            // Assert that the bins list is not null or empty
            Assert.IsNotNull(bins);

            // Assert that the bins list has three elements
            Assert.AreEqual(2, bins.Count);

            // Assert that the first bin has id 1, location "Warehouse A" and two items
            Assert.AreEqual(1, bins[0].Id);
            Assert.AreEqual("Warehouse A", bins[0].Description);
            Assert.AreEqual(2, bins[0].Items.Count);

            // Assert that the second bin has id 2, location "Warehouse B" and two items
            Assert.AreEqual(2, bins[1].Id);
            Assert.AreEqual("Warehouse B", bins[1].Description);
            Assert.AreEqual(2, bins[1].Items.Count);
        }

        [TestMethod]
        public void GetItemInBin_ValidInput_ReturnsItem()
        {
            // Arrange
            int binId = 1;
            int itemId = 1;

            // Act
            var response = _controller.GetItem(binId, itemId);
            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(content);

            var item = JsonConvert.DeserializeObject<Item>(content);

            // Assert that the bins list is not null or empty
            Assert.IsNotNull(item);

            // Assert that the first bin has id 1, location "Warehouse A" and two items
            Assert.AreEqual(itemId, item.Id);
            Assert.AreEqual("Screwdriver", item.Description);
        }

        [DataTestMethod]
        [DataRow(1, 2, -1, HttpStatusCode.BadRequest)]
        [DataRow(1, 2, 0, HttpStatusCode.OK)]
        [DataRow(1, 2, 1, HttpStatusCode.OK)]
        [DataRow(1, 2, 2, HttpStatusCode.OK)]
        [DataRow(1, 5, 2, HttpStatusCode.NotFound)]
        [DataRow(5, 2, 2, HttpStatusCode.NotFound)]
        public void AdjustItemCount_VariousInputs_ReturnsCorrectStatus(int binId, int itemId, int newCount, HttpStatusCode expectedResult)
        {
            // Act
            var response = _controller.AdjustItemCount(binId, itemId, newCount);

            // Assert 
            Assert.IsNotNull(response);
            Assert.AreEqual(expectedResult, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(content);

            // ToDo assert the NotFound responses have the correct complaint
            // ToDo get the items again and assert they have the correct new values - hint: they won't because I'm not actually saving.
        }

        [TestMethod]
        public void GetBin_ValidId_ReturnsBin()
        {
            // Arrange
            int expectedBinId = 1;
            string expectedBinDescription = "Warehouse A";
            int expectedItemCount = 2;

            // Act
            var response = _controller.GetBin(1);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(content);

            var bin = JsonConvert.DeserializeObject<Bin>(content);

            // Assert 
            Assert.IsNotNull(bin);

            Assert.AreEqual(expectedBinId, bin.Id);
            Assert.AreEqual(expectedBinDescription, bin.Description);
            Assert.AreEqual(expectedItemCount, bin.Items.Count);
        }

        [TestMethod]
        public void GetBin_InvalidId_ReturnsNotFound()
        {
            // Act
            var response = _controller.GetBin(9);

            Assert.IsNotNull(response);

            // Assert that the response status code is NotFound
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void PostBin_Valid_ReturnsBinId()
        {
            // Arrange - Create a new bin 
            int newBinId = 3;
            var newBin = new Bin()
            {
                Id = newBinId,
                Description = "Warehouse C",
                Items = new List<Item>()
                {
                    new Item() { Id = 5, Description = "Wrench", Quantity = 4 }
                }
            };

            // Act - Call the PostBin method with the new bin object and get the response
            var response = _controller.PostBin(newBin);

            // Assert 
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(content);
            var id = JsonConvert.DeserializeObject<int>(content);
            Assert.AreEqual(newBinId, id);
        }

        [TestMethod]
        public void PostBin_InvalidBin_ReturnsBadRequest()
        {
            // Create a null bin object
            Bin newBin = null;

            // Call the PostBin method with the null bin object and get the response
            var response = _controller.PostBin(newBin);

            // Assert that the response is not null
            Assert.IsNotNull(response);

            // Assert that the response status code is BadRequest
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void PostItem_ValidItem_ReturnsItemId()
        {
            // Arrange - Create a new item 
            int newItemId = 5;
            var newItem = new Item()
            {
                Id = newItemId,
                Description = "Ladder",
                Quantity = 11
            };

            // Act - Call the PostBin method with the new bin object and get the response
            var response = _controller.PostItem(2, newItem);

            // Assert 
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(content);
            var id = JsonConvert.DeserializeObject<int>(content);
            Assert.AreEqual(newItemId, id);
        }

        [TestMethod]
        public void DeleteItem_ValidItem_ReturnsNoContent()
        {
            int binId = 1;
            int itemId = 2;
            // delete item
            var deleteResponse = _controller.DeleteItemFromBin(binId, itemId);
            Assert.IsNotNull(deleteResponse);
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = _controller.GetItems(binId);
            Assert.IsNotNull(getResponse);

            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var content = getResponse.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(content);
            var items = JsonConvert.DeserializeObject<List<Item>>(content);

            Assert.Fail("This fails because the DeleteItemFromBin doesn't actaully save.");
            Assert.AreEqual(1, items.Count);
            Assert.AreNotEqual(itemId, items[0].Id);
        }

        [TestMethod]
        public void GetItems_WithValidId_ReturnsItems()
        {
            // Arrange
            int binId = 1;

            // Act
            HttpResponseMessage response = _controller.GetItems(binId);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(content);
            var items = JsonConvert.DeserializeObject<List<Item>>(content);
            Assert.AreEqual(2, items.Count);
            // ToDo More asserts!
        }
    }
}