using Moq.AutoMock;

namespace Rsp.RtsService.UnitTests;

public class TestServiceBase
{
    public AutoMocker Mocker { get; }

    public TestServiceBase()
    {
        Mocker = new AutoMocker();
    }
}