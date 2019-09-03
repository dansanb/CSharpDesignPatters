using System;
using System.Collections.Generic;
using Open_Close;

// The Open/Closed Principle states that a class should be open
// for extention, but closed for modification. We should be able to
// extend the functionality of a class, but we should not be able to
// modify it.

namespace Open_Close_Wrong
{   
    static class ProductFilter
    {
        // filter by color
        public static IEnumerable<Open_Close.Product> FilterByColor(Open_Close.Product[] products, Open_Close.Color color)
        {
            foreach (var p in products)
                if (p.color == color)
                    yield return p;
        }

        // filter by size
        public static IEnumerable<Open_Close.Product> FilterBySize(Open_Close.Product[] products, Open_Close.Size size)
        {
            foreach (var p in products)
                if (p.size == size)
                    yield return p;
        }
    }
}

namespace Open_Close_Right
{

    interface ISpeficiation<T>
    {
        bool IsSatisfied(T item);
    }

    interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> items, ISpeficiation<T> spec);
    }

    class ColorFilterSpec : ISpeficiation<Open_Close.Product>
    {
        private Color color;

        public ColorFilterSpec(Color color)
        {
            this.color = color;
        }

        public bool IsSatisfied(Product item)
        {
            return this.color == item.color;
        }
    }

    class SizeFilterSpec : ISpeficiation<Open_Close.Product>
    {
        private Size size;

        public SizeFilterSpec(Size size)
        {
            this.size = size;
        }

        public bool IsSatisfied(Product item)
        {
            return this.size == item.size;
        }
    }

    class AndFilterSpec : ISpeficiation<Open_Close.Product>
    {
        private ISpeficiation<Open_Close.Product> filter1;
        private ISpeficiation<Open_Close.Product> filter2;

        public AndFilterSpec(ISpeficiation<Open_Close.Product> filter1,
                             ISpeficiation<Open_Close.Product> filter2)
        {
            this.filter1 = filter1;
            this.filter2 = filter2;
        }

        public bool IsSatisfied(Product item)
        {
            return filter1.IsSatisfied(item) && filter2.IsSatisfied(item);
        }
    }



    class BetterFilter : IFilter<Product>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpeficiation<Product> spec)
        {
            foreach (var p in items)
                if (spec.IsSatisfied(p))
                    yield return p;
        }
    }
}

namespace Open_Close
{
    enum Size
    {
        Small, Medium, Large, Yuge
    }

    enum Color
    {
        Green, Red, Blue
    }

    class Product
    {
        public Size size { get; set; }
        public Color color { get; set; }
        public string name { get; set; }

        public Product(string name, Size size, Color color)
        {
            this.name = name;
            this.color = color;
            this.size = size;
        }

        public override string ToString()
        {
            return $"{name} has a color {color} and is {size}";
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            // add some products to an array
            var apple = new Product("Apple", Size.Small, Color.Green);
            var book = new Product("Harry Potter", Size.Medium, Color.Blue);
            var truck = new Product("Ford F150", Size.Large, Color.Red);
            var house = new Product("Mansion", Size.Yuge, Color.Blue);
            Product[] products = { apple, book, truck, house };


            // Suppose boss asks us to filter these products by color, red, easy enough:
            Console.WriteLine("Filter By Color Red: ********");
            foreach (var p in Open_Close_Wrong.ProductFilter.FilterByColor(products, Color.Red))
            {
                Console.WriteLine(p);
            }

            // Suppose boss now asks us to filter these products by size as well, medium:
            Console.WriteLine("Filter By Size Medium: ********");
            foreach (var p in Open_Close_Wrong.ProductFilter.FilterBySize(products, Size.Medium))
            {
                Console.WriteLine(p);
            }

            // Suppose boss now asks us to filter by size AND something else, like name?
            // If we keep using the Open_Close_Wrong.ProductFilter class, we are going to
            // be modifying it everytime a new combination of filters need to be made.
            // This violates the "closed for modification", since we need to modify the
            // filter class everytime a change is needed. Instead, we need to find a way
            // to extend filtering.

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Better Filter: Blue Items");



            // Using Open_Close_Right, let's filter by blue
            var betterFilter = new Open_Close_Right.BetterFilter();
            foreach (var p in betterFilter.Filter(products, new Open_Close_Right.ColorFilterSpec(Color.Blue)))
            {
                Console.WriteLine(p);
            }

            // now by size
            Console.WriteLine("Better Filter: Size Yuge");
            foreach (var p in betterFilter.Filter(products, new Open_Close_Right.SizeFilterSpec(Size.Yuge)))
            {
                Console.WriteLine(p);
            }

            // now boss says "let's filter by color AND size"
            // at this point, we extend the IFilter class and make an "AND" class to join 2 criterias together
            // note that we didn't modify any class, we simply extended it.
            //
            // Lets filter items that are BLUE and MEDIUM
            Console.WriteLine("Better Filter: Color Blue and Size Medium");
            foreach (var p in betterFilter.Filter(products, new Open_Close_Right.AndFilterSpec(
                new Open_Close_Right.ColorFilterSpec(Color.Blue),
                new Open_Close_Right.SizeFilterSpec(Size.Medium)
                )))
            {
                Console.WriteLine(p);
            }

            // If boss comes in with extra wild filtering ideas, all we have to do
            // is make a new class that extends ISpeficiation to filter by any
            // criteria.


        }
    }

}
