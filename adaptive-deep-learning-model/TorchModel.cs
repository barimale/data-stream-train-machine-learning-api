using adaptive_deep_learning_model;
using System.Text;

namespace SlowTrainMachineLearningAPI.Model
{
    public class TorchModel
    {
        private Trivial model = new Trivial();
        public Trivial Model
        {
            get
            {
                return this.model;
            }
            set
            {
                this.model = value;
            }
        }

        public void LoadFromDB(string version = "latest")
        {
            string filePath = "example.bin"; // FROM DB
            try
            {
                // Create a file to write to
                using (MemoryStream fs = new MemoryStream())
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    this.Model = new Trivial(Model.load(reader));
                    // TO DB
                }

            }
            catch (Exception)
            {
            }

            this.Model = new Trivial();
        }

        public Task<bool> SaveToDB()
        {
            string filePath = "example.bin";

            // Create a file to write to
            using (MemoryStream fs = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                Model.save(writer);
                string result = Encoding.UTF8.GetString(fs.ToArray());
                // TO DB
            }

            return Task.FromResult(true);
        }
    }
}
