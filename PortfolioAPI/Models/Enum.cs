using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetType
{
    /// <summary>Stock asset type.</summary>
    Stock,

    /// <summary>Bond asset type.</summary>
    Bond,

    /// <summary>Fund asset type.</summary>
    Fund
}

/// <summary>Types of transactions that can be made on assets.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionType
{
    /// <summary>Buying transaction.</summary>
    Buy,

    /// <summary>Selling transaction.</summary>
    Sell
}

/// <summary>Types of bonds available.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BondType
{
    /// <summary>Government bond.</summary>
    Government,

    /// <summary>Corporate bond.</summary>
    Corporate,

    /// <summary>Municipal bond.</summary>
    Municipal
}

/// <summary>Types of investment funds.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FundType
{
    /// <summary>Exchange-Traded Fund.</summary>
    ETF,

    /// <summary>Mutual Fund.</summary>
    Mutual,

    /// <summary>Index Fund.</summary>
    Index,

    /// <summary>Hedge Fund.</summary>
    Hedge,

    /// <summary>Other types of funds.</summary>
    Other
}
