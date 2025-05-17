using static TorchSharp.torch;
using TorchSharp;
using System.Globalization;
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

        private bool IsCuda = torch.cuda.is_available();
        private nn.Module<Tensor, Tensor> getLin1a(int inputLength)
        {

            return IsCuda ? nn.Linear(inputLength, 100, dtype: torch.float64).cuda() : nn.Linear(inputLength, 100, dtype: torch.float64);
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

        public nn.Module<Tensor, Tensor> merge(params nn.Module<Tensor, Tensor>[] ms)
        {
            var combinedModel = new CombinedModel(ms);

            return combinedModel;
        }

        public override Tensor forward(Tensor input)
        {
            // model switch
            if (IsCuda)
            {
                if (input.real.NumberOfElements == 5)
                {
                    using var xx = lin1.cuda().forward(input);
                    using var yy = nn.functional.relu(xx).cuda();
                    return lin2.cuda().forward(yy);
                }
                else if (input.real.NumberOfElements == 10)
                {
                    using var x = lin1b.cuda().forward(input);
                    using var y = nn.functional.relu(x).cuda();
                    return lin2.cuda().forward(y);
                }

                // dynamic/adaptive input layer
                using var seq = nn.Sequential(
                    ("lin1", getLin1a((int)input.real.NumberOfElements)),
                    ("relu1", nn.ReLU()),
                    ("drop1", nn.Dropout(0.1)),
                    ("lin2", lin2),
                    ("relu2", nn.ReLU())).cuda();

                return seq.forward(input);
            }
            else
            {
                if (input.real.NumberOfElements == 5)
                {
                    using var xx = lin1.forward(input);
                    using var yy = nn.functional.relu(xx);
                    return lin2.forward(yy);
                }
                else if (input.real.NumberOfElements == 10)
                {
                    using var x = lin1b.forward(input);
                    using var y = nn.functional.relu(x);
                    return lin2.forward(y);
                }

                // dynamic/adaptive input layer
                using var seq = nn.Sequential(
                    ("lin1", getLin1a((int)input.real.NumberOfElements)),
                    ("relu1", nn.ReLU()),
                    ("drop1", nn.Dropout(0.1)),
                    ("lin2", lin2),
                    ("relu2", nn.ReLU()));

                return seq.forward(input);
            }
        }

        public Tensor? TransformInputData(params double[] numbers)
        {
            return IsCuda ? torch.from_array(numbers, dtype: torch.float64).cuda() : torch.from_array(numbers, dtype: torch.float64);
        }

        public Tensor TransformInputData(params string[] numbers)
        {
            var tensors = new Tensor[numbers.Length];
            var index = 0;

            foreach (var s in numbers)
            {
                string[] s1 = s.Trim('[', ']').Split(',');
                double[] myArr = Array.ConvertAll(s1, n => double.Parse(n, CultureInfo.InvariantCulture));
                tensors[index] = TransformInputData(myArr);
                index += 1;
            }

            return IsCuda ? torch.stack(tensors).cuda() : torch.stack(tensors);
        }

        public Tensor? predict(Tensor? dataBatch)
        {
            var result = this.forward(dataBatch);
            return result;
        }

        public double train(Tensor? dataBatch, Tensor? resultBatch)
        {
            // to be customized / adaptive
            //var learning_rate = 0.001f; adaptive via Adam
            // to be customized / adaptive
            MSELoss loss = IsCuda ? nn.MSELoss().cuda() : nn.MSELoss();
            // to be customized / adaptive
            var EPOCHS = 3;
            var finalLoss = 0.0d;
            var steps = 500;
            // to be customized / adaptive
            var optimizer = torch.optim.Adam(this.parameters());

            for (int e = 0; e < EPOCHS; e++)
            {
                for (int i = 0; i < steps; i++)
                {
                    // Compute the loss
                    using var output = loss.forward(this.forward(dataBatch), resultBatch);

                    // Clear the gradients before doing the back-propagation
                    this.zero_grad();

                    torch.autograd.set_detect_anomaly(true);

                    // Do back-progatation, which computes all the gradients.
                    output.backward();

                    optimizer.step();
                }

                finalLoss = loss.forward(this.forward(dataBatch), resultBatch).item<double>();
            }

            return finalLoss;
        }
    }
}
