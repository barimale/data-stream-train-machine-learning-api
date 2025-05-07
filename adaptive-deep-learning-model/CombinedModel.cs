using static TorchSharp.torch;

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

        public override Tensor? forward(Tensor input)
        {
            if(models.Length == 0)
            {
                return null;
            }
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
}
