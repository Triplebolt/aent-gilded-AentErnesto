using System.Collections.Generic;

namespace GildedRoseKata;

public class GildedRose
{
    IList<Item> Items;

    public GildedRose(IList<Item> Items)
    {
        this.Items = Items;
    }

    private const string AgedBrie = "Aged Brie";
    private const string Backstage = "Backstage passes to a TAFKAL80ETC concert";
    private const string Sulfuras = "Sulfuras, Hand of Ragnaros";

    public void UpdateQuality()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i].Name == Sulfuras)
            {
                UpdateSulfuras(Items[i]);
                continue;
            }

            if (Items[i].Name == AgedBrie)
            {
                UpdateAgedBrie(Items[i]);
                continue;
            }

            if (Items[i].Name == Backstage)
            {
                UpdateBackstagePass(Items[i]);
                continue;
            }

            UpdateNormalItem(Items[i]);
        }
    }

    // Ordinary goods: degrade by 1, or by 2 once past the sell-by date; never below 0.
    private static void UpdateNormalItem(Item item)
    {
        if (item.Quality > 0)
        {
            item.Quality = item.Quality - 1;
        }

        item.SellIn = item.SellIn - 1;

        if (item.SellIn < 0)
        {
            if (item.Quality > 0)
            {
                item.Quality = item.Quality - 1;
            }
        }
    }

    // Legendary: never changes quality and never has to be sold.
    private static void UpdateSulfuras(Item item)
    {
    }

    // Increases in quality as it ages; twice as fast once past its sell-by date.
    private static void UpdateAgedBrie(Item item)
    {
        if (item.Quality < 50)
        {
            item.Quality = item.Quality + 1;
        }

        item.SellIn = item.SellIn - 1;

        if (item.SellIn < 0)
        {
            if (item.Quality < 50)
            {
                item.Quality = item.Quality + 1;
            }
        }
    }

    // Increases in quality as the concert nears (+2 at <=10 days, +3 at <=5 days),
    // then drops to 0 once the concert has passed.
    private static void UpdateBackstagePass(Item item)
    {
        if (item.Quality < 50)
        {
            item.Quality = item.Quality + 1;

            if (item.SellIn < 11)
            {
                if (item.Quality < 50)
                {
                    item.Quality = item.Quality + 1;
                }
            }

            if (item.SellIn < 6)
            {
                if (item.Quality < 50)
                {
                    item.Quality = item.Quality + 1;
                }
            }
        }

        item.SellIn = item.SellIn - 1;

        if (item.SellIn < 0)
        {
            item.Quality = 0;
        }
    }
}