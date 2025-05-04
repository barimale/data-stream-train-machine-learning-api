using static TorchSharp.torch;
using TorchSharp;

namespace adaptive_deep_learning_model
{

    public class Trivial : nn.Module<Tensor, Tensor>
    {
        public Trivial(nn.Module module)
            : base(nameof(Trivial))
        {
            this.register_module(nameof(Trivial), module);
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

        public Tensor? TransformInputData(params float[] numbers)
        {
            var tensor = torch.from_array(numbers);
            return tensor;
        }

        public Tensor? TransformInputData(params string[] numbers)
        {
            float[] myArr = Array.ConvertAll(numbers, n => float.Parse(n));
            return TransformInputData(myArr);
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

        private nn.Module<Tensor, Tensor> lin1 = nn.Linear(5, 100);
        private nn.Module<Tensor, Tensor> lin2 = nn.Linear(100, 10);
    }
}
