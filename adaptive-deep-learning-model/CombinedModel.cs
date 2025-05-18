using TorchSharp;
using static TorchSharp.torch;

namespace adaptive_deep_learning_model
{
    // merge models
    public class CombinedModel : nn.Module<Tensor, Tensor>
    {
        private nn.Module<Tensor, Tensor>[] models;
		private Device Device = torch.cuda.is_available() ? torch.CUDA : torch.CPU;

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
                outputs[index] = model.forward(input).to(Device);
                index++;
            }

            Tensor final = outputs[0];
            foreach (var t in outputs.Skip(1))
            {
                final = (final + t)/2;
            }

            var yy = nn.functional.relu(final).to(Device); // models.Length); WIP
            return yy;
        }
    }
}
