namespace Shared.CommonTypes.Geo;

public record Route(IList<Path> Steps)
{
    public Position Start => Steps.FirstOrDefault()?.From ?? throw new Exception("Empty Route");
    public Position End => Steps.LastOrDefault()?.To ?? throw new Exception("Empty Route");

    public double Length => Steps.Sum(x => x.Length);
}