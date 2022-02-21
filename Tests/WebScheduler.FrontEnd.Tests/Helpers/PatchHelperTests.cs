namespace WebScheduler.FrontEnd.Tests.Helpers;
using FluentAssertions;
using WebScheduler.FrontEnd.BlazorApp.Helpers;
using Xunit;

/// <summary>
/// See https://gist.github.com/yww325/b71563462cb5b5f2ea29e0143634bebe
/// and https://stackoverflow.com/a/50011301/103302
/// </summary>
public class PatchHelperTests
{
    [Fact]
    public void ShouldReportRemoveArrayObjectElement()
    {
        dynamic a = new
        {
            array = new dynamic[]
            {
                new
                {
                    P="a"
                },
                new
                {
                    P="b"
                },
            }
        };

        dynamic b = new
        {
            array = new dynamic[]
            {
                new
                {
                    P="a"
                },
            }
        };

        var patch = PatchHelper.CompareObjects((object)a, (object)b);
        patch.Operations.Count.Should().Be(1);
        patch.Operations.Should().Contain(o => o.op == "remove"
                                               && o.path == "/array/1"
                                                );
    }

    [Fact]
    public void ShouldReportReplaceAndRemoveArrayObjectElement()
    {
        dynamic a = new
        {
            array = new dynamic[]
            {
                new
                {
                    P="a"
                },
                new
                {
                    P="b"
                },
            }
        };

        dynamic b = new
        {
            array = new dynamic[]
            {
                new
                {
                    P="b"
                },
            }
        };

        var patch = PatchHelper.CompareObjects((object)a, (object)b);
        patch.Operations.Count.Should().Be(2);
        patch.Operations.Should().Contain(o => o.op == "replace"
                                               && o.path == "/array/0/P"
                                               && o.value.ToString() == "b"
        );
        patch.Operations.Should().Contain(o => o.op == "remove"
                                               && o.path == "/array/1"
        );
    }

    [Fact]
    public void ShouldReportRemoveArraySimpleElement()
    {
        dynamic a = new
        {
            array = new dynamic[]
            {
                "a",
                "b"
            }
        };

        dynamic b = new
        {
            array = new dynamic[]
            {
                "a"
            }
        };

        var patch = PatchHelper.CompareObjects((object)a, (object)b);
        patch.Operations.Count.Should().Be(1);
        patch.Operations.Should().Contain(o => o.op == "remove"
                                               && o.path == "/array/1"
        );
    }

    [Fact]
    public void ShouldReportReplaceAndRemoveArraySimpleElement()
    {
        dynamic a = new
        {
            array = new dynamic[]
            {
                "a",
                "b"
            }
        };

        dynamic b = new
        {
            array = new dynamic[]
            {
                "b"
            }
        };

        var patch = PatchHelper.CompareObjects((object)a, (object)b);
        patch.Operations.Count.Should().Be(2);
        patch.Operations.Should().Contain(o => o.op == "replace"
                                               && o.path == "/array/0"
                                               && o.value.ToString() == "b"
        );
        patch.Operations.Should().Contain(o => o.op == "remove"
                                               && o.path == "/array/1"
        );
    }

    [Fact]
    public void ShouldReportAddArraySimpleElement()
    {
        dynamic a = new
        {
            array = new dynamic[]
            {
                "a",
                "b"
            }
        };

        dynamic b = new
        {
            array = new dynamic[]
            {
                "a",
                "b",
                "c"
            }
        };

        var patch = PatchHelper.CompareObjects((object)a, (object)b);
        patch.Operations.Count.Should().Be(1);
        patch.Operations.Should().Contain(o => o.op == "add"
                                               && o.path == "/array/-"
                                               && o.value.ToString() == "c"
        );
    }

    [Fact]
    public void ShouldReportRemoveAndAddProperty()
    {
        dynamic a = new
        {
            pa = 1
        };

        dynamic b = new
        {
            pb = 2
        };

        var patch = PatchHelper.CompareObjects((object)a, (object)b);
        patch.Operations.Count.Should().Be(2);
        patch.Operations.Should().Contain(o => o.op == "remove"
                                               && o.path == "/pa"
        );
        patch.Operations.Should().Contain(o => o.op == "add"
                                               && o.path == "/pb"
                                               && (int)o.value == 2
        );
    }

    [Fact]
    public void ShouldReportReplaceWithChangingType()
    {
        dynamic a = new
        {
            pa = 1
        };

        dynamic b = new
        {
            pa = "hello"
        };

        var patch = PatchHelper.CompareObjects((object)a, (object)b);
        patch.Operations.Count.Should().Be(1);
        patch.Operations.Should().Contain(o => o.op == "replace"
                                               && o.path == "/pa"
                                               && (string)o.value == "hello"
        );
    }

    [Fact]
    public void ShouldReportReplaceWithDeepLayer()
    {
        dynamic a = new
        {
            pa = new
            {
                paa = new
                {
                    paaa = 123,
                    paab = 123
                }
            }
        };

        dynamic b = new
        {
            pa = new
            {
                paa = new
                {
                    paaa = 123,
                    paab = 456
                }
            }
        };

        var patch = PatchHelper.CompareObjects((object)a, (object)b);
        patch.Operations.Count.Should().Be(1);
        patch.Operations.Should().Contain(o => o.op == "replace"
                                               && o.path == "/pa/paa/paab"
                                               && (int)o.value == 456
        );
    }
}
