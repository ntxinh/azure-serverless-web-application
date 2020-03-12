using System.Collections.Generic;

namespace Infrastructure.Services
{
    public class BlobStorageOptions
    {
        #region Variables

        public string ConnectionString { get; set; }
        public IDictionary<string, string> Containers { get; set; }

        #endregion

        #region Constructors

        public BlobStorageOptions()
        {
            // Set values for instance variables
            this.ConnectionString = "";
            this.Containers = new Dictionary<string, string>();

        } // End of the constructor

        #endregion

    } // End of the class
}
