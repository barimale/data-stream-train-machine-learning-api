using adaptive_deep_learning_model;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace SlowTrainMachineLearningAPI.Model
{
    public class TorchModel
    {
        private readonly ISender _sender;
        public TorchModel(IServiceProvider serviceProvider)
        {
            _sender = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ISender>();
        }

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
            byte[] modelFromDb = new byte[2];// FROM DB
            try
            {
                using (MemoryStream fs = new MemoryStream(modelFromDb))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    this.Model = new Trivial(Model.load(reader));
                    return;
                }

            }
            catch (Exception)
            {
            }

            this.Model = new Trivial();
        }

        public Task<bool> SaveToDB()
        {
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
