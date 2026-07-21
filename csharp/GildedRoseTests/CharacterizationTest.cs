using System.Collections.Generic;

using GildedRoseKata;

using Xunit;

namespace GildedRoseTests;

// Characterization tests: these lock in what UpdateQuality does TODAY, not what
// the requirements say it should do. They are a safety net for refactoring — if a
// change alters observed behavior, one of these will fail. Every assertion here was
// derived by tracing the current code, and all pass against the unchanged implementation.
public class CharacterizationTest
{
    private const string Backstage = "Backstage passes to a TAFKAL80ETC concert";
    private const string Sulfuras = "Sulfuras, Hand of Ragnaros";

    // Runs a single item through one UpdateQuality tick and returns it.
    private static Item AfterOneUpdate(string name, int sellIn, int quality)
    {
        IList<Item> items = new List<Item> { new Item { Name = name, SellIn = sellIn, Quality = quality } };
        new GildedRose(items).UpdateQuality();
        return items[0];
    }

    [Fact]
    public void NormalItem_DegradesQualityAndSellInByOne()
    {
        var item = AfterOneUpdate("+5 Dexterity Vest", sellIn: 10, quality: 20);
        Assert.Equal(9, item.SellIn);
        Assert.Equal(19, item.Quality);
    }

    [Fact]
    public void NormalItem_PastSellByDate_DegradesQualityTwiceAsFast()
    {
        // SellIn already at 0: after the tick it is negative, so quality drops by 2 (10 -> 8),
        // not 1. Starting quality is well above 0 so the floor never masks the magnitude.
        var item = AfterOneUpdate("+5 Dexterity Vest", sellIn: 0, quality: 10);
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(8, item.Quality);
    }

    [Fact]
    public void AgedBrie_IncreasesInQualityAsItAges()
    {
        var item = AfterOneUpdate("Aged Brie", sellIn: 2, quality: 0);
        Assert.Equal(1, item.SellIn);
        Assert.Equal(1, item.Quality);
    }

    [Fact]
    public void Sulfuras_NeverChangesQualityOrSellIn()
    {
        var item = AfterOneUpdate(Sulfuras, sellIn: 0, quality: 80);
        Assert.Equal(0, item.SellIn);
        Assert.Equal(80, item.Quality);
    }

    [Fact]
    public void BackstagePass_MoreThanTenDaysOut_IncreasesByOne()
    {
        var item = AfterOneUpdate(Backstage, sellIn: 15, quality: 20);
        Assert.Equal(14, item.SellIn);
        Assert.Equal(21, item.Quality);
    }

    [Fact]
    public void BackstagePass_TenDaysOrFewer_IncreasesByTwo()
    {
        var item = AfterOneUpdate(Backstage, sellIn: 10, quality: 20);
        Assert.Equal(9, item.SellIn);
        Assert.Equal(22, item.Quality);
    }

    [Fact]
    public void BackstagePass_FiveDaysOrFewer_IncreasesByThree()
    {
        var item = AfterOneUpdate(Backstage, sellIn: 5, quality: 20);
        Assert.Equal(4, item.SellIn);
        Assert.Equal(23, item.Quality);
    }

    [Fact]
    public void BackstagePass_AfterTheConcert_DropsToZero()
    {
        var item = AfterOneUpdate(Backstage, sellIn: 0, quality: 20);
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void Quality_NeverGoesBelowZero()
    {
        // Normal item already at 0, past its sell date (would otherwise degrade by 2).
        var item = AfterOneUpdate("+5 Dexterity Vest", sellIn: 0, quality: 0);
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void Quality_NeverExceedsFifty()
    {
        // Aged Brie at the cap stays at the cap.
        var item = AfterOneUpdate("Aged Brie", sellIn: 2, quality: 50);
        Assert.Equal(1, item.SellIn);
        Assert.Equal(50, item.Quality);
    }
}
