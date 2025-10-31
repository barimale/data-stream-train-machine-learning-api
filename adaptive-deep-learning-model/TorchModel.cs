using Application.CQRS.Commands;
using Application.CQRS.Queries;
using MediatR;

namespace adaptive_deep_learning_model
{
    public class TorchModel : ITorchModel
    {
        private readonly ISender _sender;
        public TorchModel(ISender sender)
        {
            _sender = sender;
        }

        public bool IsTrainingInProgress { get; set; } = false;

        private Trivial model = new Trivial();
        public Trivial Model
        {
            get
            {
                return model;
            }
            private set
            {
                model = value;
            }
        }

        public async Task<CombinedModel> GetModelFromPieces(GetModuleResult mainModel)
        {
            var result = await _sender.Send(new GetPiecesQuery());

            if (mainModel is not null && mainModel.Model is not null)
            {
                var pieces = result.Models.Select(p => p.PieceOfModel);
                var trivials = new Trivial[pieces.Count() + 1];

                using (MemoryStream fs = new MemoryStream(mainModel.Model.ModelAsBytes))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    var trivial = new Trivial();
                    trivial.load(reader);
                    trivials[0] = trivial;
                }

                var index = 1;
                foreach (var submodule in pieces)
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
            else
            {
                var pieces = result.Models.Select(p => p.PieceOfModel);
                var trivials = new Trivial[pieces.Count()];

                var index = 0;
                foreach (var submodule in pieces)
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

        }

        public async Task LoadFromDB(string version = "latest")
        {
            var result = await _sender.Send(new GetLatestQuery(version));

            if (result.Model is null)
            {
                Model = new Trivial();
                return;
            }

            byte[] modelFromDb = result.Model.ModelAsBytes;
            try
            {
                using (MemoryStream fs = new MemoryStream(modelFromDb))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    Model.load(reader);
                    return;
                }

            }
            catch (Exception)
            {
            }

            Model = new Trivial();
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

        public byte[] ModelToBytes(Trivial model)
        {
            using (MemoryStream fs = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                model.save(writer);
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
