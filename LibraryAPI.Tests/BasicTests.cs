using Xunit;

namespace LibraryAPI.Tests
{
    public class BasicTests
    {
        [Fact]
        public void Test_Addition()
        {
            Assert.Equal(4, 2 + 2);
        }
        
        [Fact]
        public void Test_AlwaysPasses()
        {
            Assert.True(true);
        }
    }
}
