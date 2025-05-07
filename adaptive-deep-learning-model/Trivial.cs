using static TorchSharp.torch;
using TorchSharp;

namespace adaptive_deep_learning_model
{
    // merge models
    public class CombinedModel : nn.Module<Tensor, Tensor>
    {
        private nn.Module<Tensor, Tensor>[] models;

        public CombinedModel(params nn.Module<Tensor, Tensor>[] models) : base("CombinedModel")
        {
            this.models = models;
            RegisterComponents();
        }

        public override Tensor forward(Tensor input)
        {
            Tensor[] outputs = new Tensor[models.Length];
            int index = 0;
            foreach (var model in models)
            {
                outputs[index] = model.forward(input);
                index++;
            }

            Tensor final = outputs[0];
            foreach (var t in outputs.Skip(1))
            {
                final = final + t;
            }
            var yy = nn.functional.relu(final / models.Length);
            return yy;
        }
    }

    // probably to be replaced by a RNN or CNN model
    // TODOs: https://pytorch.org/blog/computational-graphs-constructed-in-pytorch/
    public class Trivial : nn.Module<Tensor, Tensor>
    {
        // to be customized / adaptive
        private nn.Module<Tensor, Tensor> lin1 = nn.Linear(5, 100);
        private nn.Module<Tensor, Tensor> lin1b = nn.Linear(10, 100);
        private nn.Module<Tensor, Tensor> lin2 = nn.Linear(100, 10);

        private nn.Module<Tensor, Tensor> getLin1a(int inputLength)
        {
            return nn.Linear(inputLength, 100);
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

        public Tensor? TransformInputData(params float[] numbers)
        {
            var tensor = torch.from_array(numbers);
            return tensor;
        }

        public Tensor TransformInputData(params string[] numbers)
        {
            var tensors = new Tensor[numbers.Length];
            var index = 0;

            foreach (var s in numbers)
            {
                string[] s1 = s.Trim('[', ']').Split(',');
                float[] myArr = Array.ConvertAll(s1, n => float.Parse(n));
                tensors[index] = TransformInputData(myArr);
                index += 1;
            }

            Tensor states = torch.stack(tensors);
            return states;
        }

        public Tensor? predict(Tensor? dataBatch)
        {
            var result = this.forward(dataBatch);
            return result;
        }

        public float train(Tensor? dataBatch, Tensor? resultBatch)
        {
            // to be customized / adaptive
            //var learning_rate = 0.001f; adaptive via Adam
            // to be customized / adaptive
            var loss = nn.MSELoss();
            // to be customized / adaptive
            var EPOCHS = 3;
            var finalLoss = 0.0f;
            // to be customized / adaptive
            var optimizer = torch.optim.Adam(this.parameters());

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

                finalLoss = loss.forward(this.forward(dataBatch), resultBatch).item<float>();
            }

            return finalLoss;
        }
    }
}
