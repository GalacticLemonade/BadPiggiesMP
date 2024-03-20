// See https://aka.ms/new-console-template for more information
namespace BadPiggiesMP {
    public class Program {
        public static async Task Main(string[] args) {
            await new BadPiggiesServer().Main();
        }
    }
}