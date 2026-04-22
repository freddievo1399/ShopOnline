using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopOnline.Abstractions.Requests;

public class DataQueryRequest
{
    public int? Skip { get; set; } = 0;
    public int? Take { get; set; } = 0;
    public bool? RequiresCounts { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<WhereFilter>? Where { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Sort>? Sorted { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SearchFilter>? Search { get; set; }

    public List<string>? Group { get; set; }
    public List<Aggregate>? Aggregates { get; set; }

    public List<string>? Select { get; set; }
    public List<string>? Expand { get; set; }

    public IDictionary<string, object>? Params { get; set; }
    public List<string>? Distinct { get; set; }

    public string? IdMapping { get; set; }
    public string? antiForgery { get; set; }
    public IDictionary<string, string>? GroupByFormatter { get; set; }

    public string? Table { get; set; }
    public static implicit operator DataQueryRequest(DataManagerRequest request)
    {
        if (request == null) return new DataQueryRequest();
        return new DataQueryRequest
        {
            Skip = request.Skip,
            Take = request.Take,
            RequiresCounts = request.RequiresCounts,
            Where = request.Where ?? new(),
            Sorted = request.Sorted ?? new(),
            Search = request.Search ?? new(),
            Group = request.Group ?? new(),
            Aggregates = request.Aggregates ?? new(),
            Select = request.Select ?? new(),
            Expand = request.Expand ?? new(),
            Params = request.Params ?? new Dictionary<string, object>(),
            Distinct = request.Distinct ?? new(),
            IdMapping = request.IdMapping ?? "",
            antiForgery = request.antiForgery ?? "",
            GroupByFormatter = request.GroupByFormatter ?? new Dictionary<string, string>(),
            Table = request.Table ?? ""
        };
    }

    public DataManagerRequest ToRequest()
    {
        return new DataManagerRequest
        {
            Skip = this.Skip ?? 0,
            Take = this.Take ?? 0,
            RequiresCounts = this.RequiresCounts ?? false,
            Where = this.Where ?? new(),
            Sorted = this.Sorted ?? new(),
            Search = this.Search ?? new(),
            Group = this.Group ?? new(),
            Aggregates = this.Aggregates ?? new(),
            Select = this.Select ?? new(),
            Expand = this.Expand ?? new(),
            Params = this.Params ?? new Dictionary<string, object>(),
            Distinct = this.Distinct ?? new(),
            IdMapping = this.IdMapping ?? "",
            antiForgery = this.antiForgery ?? "",
            GroupByFormatter = this.GroupByFormatter ?? new Dictionary<string, string>(),
            Table = this.Table ?? ""
        };
    }
}
