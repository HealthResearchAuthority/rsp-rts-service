using Moq.AutoMock;

namespace Rts.RtsImport.UnitTests;

public class TestServiceBase
{
    public AutoMocker Mocker { get; }

    public TestServiceBase()
    {
        Mocker = new AutoMocker();
    }
}