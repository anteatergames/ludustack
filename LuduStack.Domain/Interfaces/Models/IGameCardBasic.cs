namespace LuduStack.Domain.Interfaces.Models
{
    public interface IGameCardBasic : IEntityBase
    {
        string Title { get; set; }

        string DeveloperImageUrl { get; set; }

        string DeveloperName { get; set; }

        public string DeveloperHandler { get; set; }

        string ThumbnailUrl { get; set; }

        string ThumbnailResponsive { get; set; }

        string ThumbnailLquip { get; set; }
    }
}