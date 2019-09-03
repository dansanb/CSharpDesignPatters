using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

// The single responsability principle states that a class should
// only be responsible for one thing. It's important to have lean
// classes.

namespace SingleResponsability_Wrong
{
    /// <summary>
    /// This first Journal class keeps track of entries and persistance by
    /// saving the entries to disk. It breaks the Single Responsability
    /// since it's doing too much.
    /// </summary>
    public class Journal
    {
        private List<string> entries = new List<string>();
        private int count = 0;

        public void AddEntry(string text)
        {
            entries.Add($"{++count}: {text}");

        }

        public void Remove(int index)
        {
            entries.RemoveAt(index);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, entries);
        }

        public void Save(string fileName)
        {
            File.WriteAllText(fileName, ToString());
        }

        public void Load(string fileName)
        {
            // code to load journal entries here
        }
    }
}




namespace SingleResponsabilit_Right
{
    /// <summary>
    /// This journal class is only made responsible for one thing, which
    /// is to manage journal entries. The disk operations are extracted
    /// and assigned to a separate class (Persistance) below.
    /// </summary>
    public class Journal
    {
        private List<string> entries = new List<string>();
        private int count = 0;

        public void AddEntry(string text)
        {
            entries.Add($"{++count}: {text}");

        }

        public void Remove(int index)
        {
            entries.RemoveAt(index);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, entries);
        }        
    }

    /// <summary>
    /// This class also has one responsability, and that is to take an
    /// object (in this case a Journal type) and handle persistance options,
    /// such as save and load to and from disk, url, or any other future
    /// requirement.
    /// </summary>
    public class Persistance
    {
        public void SaveJournal(string fileName, Journal j)
        {
            File.WriteAllText(fileName, j.ToString());
        }

        // can load from disk
        public Journal LoadFromDisk(string fileName)
        {
            // code to load journal entries here
            //
            // left un-implemented on purpose
            return null;
        }

        // can load from uri
        public Journal LoadFromUri(string uri)
        {
            // code to load journal entries here
            //
            // left un-implemented on purpose
            return null;
        }

    }
}

namespace SingleResponsability
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // usage of wrong journal - this journal class does too much
            // and is hard to maintain.
            var wrongJournal = new SingleResponsability_Wrong.Journal();
            wrongJournal.AddEntry("I ate a bug today");
            wrongJournal.AddEntry("I feel sad");
            Console.WriteLine("Wrong Journal =========");
            Console.WriteLine(wrongJournal);

            string wrongFileName = "/Users/daniel/Temp/wrongJournal.txt";
            wrongJournal.Save(wrongFileName);
            Process.Start(wrongFileName);

            Console.WriteLine(Environment.NewLine);


            // usage of right journal - everything is separated and
            // easily maintainable and upgradable
            var rightJournal = new SingleResponsabilit_Right.Journal();
            rightJournal.AddEntry("I ate a bug today");
            rightJournal.AddEntry("I feel sad");
            Console.WriteLine("Right Journal =========");
            Console.WriteLine(rightJournal);

            string rightFileName = "/Users/daniel/Temp/rightJournal.txt";
            var p = new SingleResponsabilit_Right.Persistance();
            p.SaveJournal(rightFileName, rightJournal);            
            Process.Start(rightFileName);
            

        }
    }
}
