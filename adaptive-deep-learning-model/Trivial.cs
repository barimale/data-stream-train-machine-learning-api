using static TorchSharp.torch;
using TorchSharp;
using TorchSharp.Modules;

namespace adaptive_deep_learning_model
{

    // probably to be replaced by a RNN or CNN model
    // TODOs: https://pytorch.org/blog/computational-graphs-constructed-in-pytorch/
    public class Trivial : nn.Module<Tensor, Tensor>
    {
        // to be customized / adaptive
        private nn.Module<Tensor, Tensor> lin1 = nn.Linear(5, 100, dtype: torch.float64);
        private nn.Module<Tensor, Tensor> lin1b = nn.Linear(10, 100, dtype: torch.float64);
        private nn.Module<Tensor, Tensor> lin2 = nn.Linear(100, 10, dtype: torch.float64);

        private Device Device = torch.cuda.is_available() ? torch.CUDA : torch.CPU;

        private nn.Module<Tensor, Tensor> getLin1a(int inputLength)
        {

            return nn.Linear(inputLength, 100, dtype: torch.float64).to(Device);
        }

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
            // model switch
            
            if (input.real.NumberOfElements == 5)
            {
                using var xx = lin1.forward(input).to(Device);
                using var yy = nn.functional.relu(xx).to(Device);
                return lin2.forward(yy).to(Device);
            }
            else if (input.real.NumberOfElements == 10)
            {
                using var x = lin1b.forward(input).to(Device);
                using var y = nn.functional.relu(x).to(Device);
                return lin2.forward(y).to(Device);
            }

            // dynamic/adaptive input layer
            using var seq = nn.Sequential(
                ("lin1", getLin1a((int)input.real.NumberOfElements)),
                ("relu1", nn.ReLU()),
                ("drop1", nn.Dropout(0.1)),
                ("lin2", lin2),
                ("relu2", nn.ReLU())).to(Device);

            return seq.forward(input).to(Device);
        }

        public Tensor? TransformInputData(params double[] numbers)
        {
            return torch.from_array(numbers, dtype: torch.float64).to(Device);
        }

        public double train(Tensor? dataBatch, Tensor? resultBatch)
        {
            // to be customized / adaptive
            //var learning_rate = 0.001f; adaptive via Adam
            // to be customized / adaptive
            MSELoss loss = nn.MSELoss().to(Device);
            // to be customized / adaptive
            var EPOCHS = 3;
            var finalLoss = 0.0d;
            var steps = 1000;
            // to be customized / adaptive
            var optimizer = torch.optim.Adam(this.parameters());

            for (int e = 0; e < EPOCHS; e++)
            {
                for (int i = 0; i < steps; i++)
                {
                    // Compute the loss
                    using var output = loss.forward(this.forward(dataBatch), resultBatch).to(Device);

                    // Clear the gradients before doing the back-propagation
                    this.zero_grad();
                    // WIP maybe not necessary at all 
                    this.to(Device);

                    torch.autograd.set_detect_anomaly(true);

                    // Do back-propagation, which computes all the gradients.
                    output.backward();

                    optimizer.step();
                }

                finalLoss = loss.forward(this.forward(dataBatch), resultBatch).to(Device).item<double>();
            }

            return finalLoss;
        }
    }
}
