using System.Text;

namespace Servus.Core.Application;

public static class Servus
{
    public static readonly string Logo = new StringBuilder()
        .AppendLine()
        .AppendLine()
        .AppendLine(@"             /\")
        .AppendLine(@"            /  \")
        .AppendLine(@"           /    \__")
        .AppendLine(@"     /\   /        \    /\")
        .AppendLine(@"    /  \_/          \__/  \")
        .AppendLine(@"   /                       \")
        .AppendLine(@"  /_________________________\")
        .AppendLine("            servus!")
        .AppendLine()
        .AppendLine()
        .ToString();

    public static readonly string LogoSmall = new StringBuilder()
        .AppendLine()
        .AppendLine()
        .AppendLine(@"           /\")
        .AppendLine(@"    /\    /  \_    /\")
        .AppendLine(@"   /  \  /      \_/  \")
        .AppendLine(@"  /    \/             \")
        .AppendLine(@" /_____________________\")
        .AppendLine( "         servus!")
        .AppendLine()
        .AppendLine().ToString();
    
    public static readonly string LogoTiny = new StringBuilder()
        .AppendLine()
        .AppendLine()
        .AppendLine("servus!")
        .AppendLine()
        .AppendLine()
        .ToString();
}