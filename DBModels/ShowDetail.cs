using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class ShowDetail
{
    public int ShowDetailsId { get; set; }

    public int? ShowDetailsShowid { get; set; }

    public int? ShowDetailsZoneId { get; set; }

    public int? ShowDetailsFileId { get; set; }

    public string? ShowDetailsShowcode { get; set; }

    public DateTime? ShowDetailsCdate { get; set; }

    public DateTime? ShowDetailsUdate { get; set; }

    public string? ShowDetailsCustCode { get; set; }

    public int? ShowDetailsIsactive { get; set; }
}
