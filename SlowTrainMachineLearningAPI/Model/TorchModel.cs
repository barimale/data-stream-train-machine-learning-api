using System.Text;
using TorchSharp;
using static TorchSharp.torch;

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

        public void LoadFromDB()
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

    public class Trivial : nn.Module<Tensor, Tensor>
    {
        public Trivial(nn.Module module)
            : base(nameof(Trivial))
        {
            RegisterComponents();
        }

        public Trivial()
            : base(nameof(Trivial))
        {
            RegisterComponents();
        }

        public override Tensor forward(Tensor input)
        {
            using var x = lin1.forward(input);
            using var y = nn.functional.relu(x);
            return lin2.forward(y);
        }

        public Tensor? TransformInputData()
        {
            return rand(32, 1000);
        }

        public Tensor? test(Tensor? dataBatch)
        {
            var result = this.forward(dataBatch);
            return result;
        }

        public void train(Tensor? dataBatch)
        {
            var learning_rate = 0.001f;
            var loss = nn.MSELoss();
            //var dataBatch = rand(32, 1000);  // Our pretend input data
            var resultBatch = rand(32, 10);  // Our pretend ground truth.
            var EPOCHS = 3;

            var optimizer = torch.optim.SGD(this.parameters(), learning_rate);

            for(int e =0; e < EPOCHS; e++)
            {
                for (int i = 0; i < 1000; i++)
                {
                    // Compute the loss
                    using var output = loss.forward(this.forward(dataBatch), resultBatch);

                    // Clear the gradients before doing the back-propagation
                    this.zero_grad();

                    // Do back-progatation, which computes all the gradients.
                    output.backward();

                    optimizer.step();
                }

                loss.forward(this.forward(dataBatch), resultBatch).item<float>();
            }

            GC.Collect();
        }

        private nn.Module<Tensor, Tensor> lin1 = nn.Linear(1000, 100);
        private nn.Module<Tensor, Tensor> lin2 = nn.Linear(100, 10);
    }
}
