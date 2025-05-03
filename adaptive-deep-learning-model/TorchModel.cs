using adaptive_deep_learning_model;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
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

        public async Task LoadFromDB(string version = "latest")
        {
            var result = await _sender.Send(new GetCardBySerialNumberQuery(Guid.NewGuid().ToString())); // query instead of command

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

        public async Task<bool> SaveToDB()
        {
            using (MemoryStream fs = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                Model.save(writer);
                var result2= await _sender.Send(new RegisterModelCommand() { Model = fs.ToArray()});
            }

            return true;
        }
    }
}
