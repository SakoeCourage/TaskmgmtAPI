namespace TaskmgmtAPI.Helpers
{
    public interface IPaginator<S>
    {
        IQueryable<S> query { get; set; }
        HttpRequest request { get; set; }
        string fullRequestUrl { get { return $"{request.Scheme}://{request.Host}{request.Path}"; } }

        string nextPageUrl { get; set; }    

        string prevPageUrl { get; set; }

        int page { get; set; }

        int pageSize { get; set; }

        IEnumerable<List<S>> ToPagedList();




    }
}
