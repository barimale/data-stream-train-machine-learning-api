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

        public async Task<CombinedModel> GetModelFromPieces()
        {
            var result = await _sender.Send(new GetPiecesQuery());

            var pieces = result.Models.Select(p => p.ModelAsBytes);
            var trivials = new Trivial[pieces.Count()];
            var index = 0;
            foreach(var submodule in pieces)
            {
                try
                {
                    using (MemoryStream fs = new MemoryStream(submodule))
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        var trivial = new Trivial();
                        trivial.load(reader);
                        trivials[index] = trivial;
                        index += 1;
                    }
                }
                catch (Exception)
                {
                }
            }
            

            return new CombinedModel(trivials);
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
                    this.Model.load(reader);
                    return;
                }

            }
            catch (Exception)
            {
            }

            this.Model = new Trivial();
        }

        public byte[] ModelToBytes()
        {
            using (MemoryStream fs = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                Model.save(writer);
                return fs.ToArray();
            }
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
                    Version = version,
                });
            }

            return true;
        }
    }
}
