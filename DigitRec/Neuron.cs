using DigitRec.ActivationFunctions;
using DigitRec.Utils;

namespace DigitRec
{
    public class Neuron
    {
        public double[] Weights { get; private set; }
        public double[] PrevWeights { get; private set; }
        public double Bias { get; private set; }
        public double Value { get; private set; }
        public double Delta { get; private set; }

        public Neuron(int inputCount)
        {
            Weights = new double[inputCount];
            PrevWeights = new double[inputCount];
            CryptoRandom rand = new CryptoRandom();
            Bias = rand.NextDouble() * 2 - 1;
            for (int i = 0; i < inputCount; i++)
            {
                Weights[i] = rand.NextDouble() * 2 - 1;
                PrevWeights[i] = 0;
            }

        }

        public double Activate(double[] inputs, IActivationFunction func)
        {
            double sum = 0;
            for (int i = 0; i < Weights.Length; i++)
            {
                sum += Weights[i] * inputs[i];
            }
            sum += Bias;
            Value = func.CalculateOutput(sum);
            return Value;
        }

        public void CalculateDelta(double expected, IActivationFunction func)
        {
            Delta = -(expected - Value) * func.CalculateDerivative(Value);
        }

        public void CalculateHiddenDeltas(double sum, IActivationFunction func)
        {
            Delta = sum * func.CalculateDerivative(Value);
        }

        public double CalculateHiddenDeltas(int index)
        {
            return Weights[index] * Delta;
        }

        public void SetWeights(double[] inputs)
        {
            Weights = inputs;
        }

        public void SetBias(double bias)
        {
            Bias = bias;
        }

        public void UpdateWeights(double[] inputs, double learningRate, double gamma)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                var deltaWeight = learningRate * Delta * inputs[i] + gamma * PrevWeights[i];
                Weights[i] -= deltaWeight;
                PrevWeights[i] = deltaWeight;
            }
            Bias -= learningRate * Delta;
        }
    }
}
