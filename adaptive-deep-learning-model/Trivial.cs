using static TorchSharp.torch;
using TorchSharp;

namespace adaptive_deep_learning_model
{

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

        public Tensor? TransformInputData(params int[] numbers)
        {
            // WIP przypilnowac ksztaltu
            var tensor = torch.from_array(numbers);

            //tensor = tensor.view(1, 1000);
            return rand(32, 1000);
        }

        public Tensor? predict(Tensor? dataBatch)
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

            for (int e = 0; e < EPOCHS; e++)
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
