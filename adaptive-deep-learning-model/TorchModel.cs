using adaptive_deep_learning_model;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using MediatR;

namespace SlowTrainMachineLearningAPI.Model
{
    public class TorchModel
    {
        private readonly ISender _sender;
        public TorchModel(ISender sender)
        {
            _sender = sender;
        }

        private Trivial model = new Trivial();
        public Trivial Model
        {
            get
            {
                //LoadFromDB().GetAwaiter();
                return this.model;
            }
            set
            {
                this.model = value;
            }
        }

        public async Task LoadFromDB(string version = "latest")
        {
            var result = await _sender.Send(new GetLatestQuery(version));

            if(result.Model is null)
            {
                this.Model = new Trivial();
                return;
            }

            byte[] modelFromDb = result.Model.ModelAsBytes;
            try
            {
                using (MemoryStream fs = new MemoryStream(modelFromDb))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    //WIP all models here
                    this.Model.load(reader);
                    return;
                }

            }
            catch (Exception)
            {
            }

            this.Model = new Trivial();
        }

        public async Task<bool> SaveToDB(string version = "-")
        {
            using (MemoryStream fs = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                Model.save(writer);
                var _ = await _sender.Send(new RegisterModelCommand()
                { 
                    Model = fs.ToArray(),
                    Version = version
                });
            }

            return true;
        }
    }
}
