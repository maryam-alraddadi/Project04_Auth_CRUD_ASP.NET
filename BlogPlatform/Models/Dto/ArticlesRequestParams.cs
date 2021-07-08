namespace BlogPlatform.Models.Dto
{
    public class ArticlesRequestParams
    {
        public string Search { get; set; }
        public int Page { get; set; } = 1;
    }
}