using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Channels;
namespace _20112020
{
    class Human
    {
        public string PIB { get; set; }
        public DateTime Birthday { get; set; }
        public int Age
        {
            get
            {
                DateTime now = DateTime.Today;
                int age = now.Year - Birthday.Year;
                if (Birthday > now.AddYears(-age)) age--;
                return age;
            }
        }
        public override string ToString()
        {
            return $"|{PIB,15}| {Age,2} | {Birthday:dd.MM.yyyy} |";
        }
    }

    class Student : Human, IComparable<Student>
    {
        public string Academy { get; set; }
        private Dictionary<string, List<int>> Journal;
        public Student(string PIB, DateTime Bday, string Acad)
        {
            this.PIB = PIB;
            Birthday = Bday;
            Academy = Acad;
            Journal = new Dictionary<string, List<int>>();
        }
        public override string ToString()
        {
            string str = $"{base.ToString()}{Academy,10}|\n";
            foreach (var subject in Journal)
            {
                // str += $"\t\t|{subject.Key,15}|{string.Join('|', subject.Value)}|\n";
                str += $"\t\t|{subject.Key,10}|{subject.Value.Average(),6:N2}|{string.Join('|', subject.Value.Select(x => $"{x,2}"))}|\n";
            }
            return str;
        }
        //public int CompareTo(object obj)
        //{
        //    if (obj is Student st)
        //        return String.Compare(PIB, st.PIB);
        //    throw new InvalidCastException();
        //}

        public void NewMark(string subject, int mark)
        {
            if (!Journal.ContainsKey(subject))
            {
                Journal.Add(subject, new List<int>());
                // Journal[subject]=new List<int>();
            }
            Journal[subject].Add(mark);
        }
        public int CompareTo(Student other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var pibComparison = string.Compare(PIB, other.PIB, StringComparison.Ordinal);
            if (pibComparison != 0) return pibComparison;
            var birthdayComparison = Birthday.CompareTo(other.Birthday);
            if (birthdayComparison != 0) return birthdayComparison;
            return string.Compare(Academy, other.Academy, StringComparison.Ordinal);
        }
    }
    internal class CmpAge : IComparer<Student>
    {
        public int Compare(Student x, Student y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(x, null)) return -1;
            return x.Age.CompareTo(y.Age);
        }
    }
    class Group
    {
        public string Name { get; set; }
        private List<Student> list = new List<Student>();
        public void AddStudent(Student st) => list.Add(st);
        //public IEnumerator<Student> GetEnumerator()
        //{
        //    return list.GetEnumerator();
        //}
        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        public IEnumerator<Student> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                // Console.WriteLine("start");
                yield return list[i];
            }
            // Console.WriteLine("end");
        }
        public IEnumerable<int> GetAges()
        {
            for (int i = 0; i < Count; i++)
                yield return list[i].Age;

        }
        public override string ToString() =>
                 $"{new string('-', 50)}\nGroup name: {Name}\n" +
                 $"{String.Join('\r', list.Select(s => s.ToString()))}" +
                 $"{new string('-', 50)}\n";

        public void Sort(IComparer<Student> comparer = null)
        {
            list.Sort(comparer);
        }
        public void Sort(Comparison<Student> comparison)
        {
            list.Sort(comparison);
        }
        public void DeleteStudent(Student st)
        {
            list.Remove(st);
        }
        public Student FindStudent(string name)
        {
            return list.Find(s => 0 == String.Compare(s.PIB, name, StringComparison.CurrentCultureIgnoreCase));
        }
        public int Count => list.Count;
    }

    class Car { }
    class Zaz : Car { }
    class Vaz : Car { }
    abstract class Engine { }
    class V4 : Engine { }
    class V8 : Engine { }
    // interface IEngine<T> - Буде інваріантність
    interface IEngine<out T>  //коваріантність
    {
        T GetEngine();
        //  T GetMotor { get; }
        //  T SetEngine(T m); error
    }
    interface ISetPiston<in T> where T : Engine  //котраваріантність
    {
        void PistonToEngine(T m);
    }
    class Block<T> : ISetPiston<T> where T : Engine
    {
        List<T> cilinder = new List<T>();
        public void PistonToEngine(T m)
        {
            cilinder.Add(m);
        }
    }

    class VW : IEngine<V4>
    {
        V4 engine = new V4();
        public V4 GetEngine()
        {
            return engine;
        }
    }
    class Fiat : IEngine<V8>
    {
        V8 engine = new V8();
        public V8 GetEngine()
        {
            return engine;
        }
    }

    class Program
    {
        static Random rnd = new Random();
        static void TestList()
        {
            Student Ivan = new Student("Ivan", new DateTime(2000, 10, 20), "IT Step");
            Console.WriteLine(Ivan);
            Console.WriteLine("----------------------------");
            Ivan.NewMark("c++", 5);
            Ivan.NewMark("c#", 12);
            Ivan.NewMark("c++", 8);
            Ivan.NewMark("c++", 3);
            Ivan.NewMark("c++", 8);
            Ivan.NewMark("c++", 6);
            Console.WriteLine(Ivan);
            Console.WriteLine("----------------------------");
        }
        static void RandMarksStudent(Student st)
        {
            for (int i = 0; i < 10; i++)
            {
                st.NewMark("php", rnd.Next(1, 13));
                st.NewMark("c++", rnd.Next(1, 13));
                st.NewMark("c#", rnd.Next(1, 13));
            }
        }
        static void TestGroup()
        {
            Group gr = new Group() { Name = "PE911" };
            Student Ivan = new Student("Ivan", new DateTime(2000, 10, 20), "IT Step");
            RandMarksStudent(Ivan);
            gr.AddStudent(Ivan);
            Student Piter = new Student("Piter", new DateTime(2012, 11, 15), "it Step");
            RandMarksStudent(Piter);
            gr.AddStudent(Piter);
            Student inna = new Student("Inna", new DateTime(2012, 11, 25), "ZHDTU");
            RandMarksStudent(inna);
            gr.AddStudent(inna);
            Student Anna = new Student("Anna", new DateTime(1999, 10, 20), "ZHATC");
            RandMarksStudent(Anna);
            gr.AddStudent(Anna);
            Student Taras = new Student("Taras", new DateTime(1986, 10, 20), "ZHDU");
            RandMarksStudent(Taras);
            gr.AddStudent(Taras);
            // gr.DeleteStudent(Taras);
            // gr.Sort();
            // gr.Sort( new CmpAge());
            // gr.Sort((x,y)=>string.Compare(x.Academy,y.Academy, StringComparison.CurrentCultureIgnoreCase));
            //gr.Sort((x,y)=>x.Age.CompareTo(y.Age));
            //  gr.Sort((x,y)=>-x.Age.CompareTo(y.Age));
            Console.WriteLine(gr);
            Student f = gr.FindStudent("Ann6a");

            // Console.WriteLine(f);
            double avgAge = 0;
            int count = gr.Count;
            foreach (Student st in gr)
                avgAge += st.Age;
            Console.WriteLine($"avgAge={avgAge / count}");
            avgAge = 0;
            count = gr.Count;
            foreach (int a in gr.GetAges())
                avgAge += a;
            Console.WriteLine($"avgAge={avgAge / count}");
            Console.WriteLine($"avgAge={gr.GetAges().Average()}");


        }
        static Human TesHuman(string n)
        {
            return new Human { PIB = n, Birthday = DateTime.Today };
        }
        static Student TesStudent(string n)
        {
            Student s = new Student(n, new DateTime(2012, 11, 25), "ZHDTU");
            RandMarksStudent(s);
            return s;
        }
        delegate Human DelegNewH(string n);
        static void TestKo() //Коваріантність
        {
            DelegNewH NewHuman = TesHuman;
            Human h = NewHuman("Ivan");
            Console.WriteLine(h);
            NewHuman = TesStudent;
            //Student s  = NewHuman("Inna");//Error
            Human s = NewHuman("Inna");
            Console.WriteLine(s);
            ///////////////////////////////////////////////////////////////////////////
            List<Zaz> zazy = new List<Zaz>();
            IEnumerable<Zaz> ienzaz = zazy;
            //  IEnumerable<Car>= IEnumerable<Zaz>
            IEnumerable<Car> iencar = ienzaz;  //zazy; //
            object[] objs = new string[5];
            objs[0] = "text";
            // objs[1] = 15;  //error
            VW Caddy = new VW();
            IEngine<V4> ien4 = Caddy;
            IEngine<Engine> ien1 = Caddy;
            IEngine<Engine> ien2 = ien4;

        }
        delegate void DelegHumanPrint(Human h);
        static void StudentPrint(Student s)
        {
            Console.WriteLine(s);
        }
        static void TestKontra() //Контраваріантність
        {
            DelegHumanPrint show = h => Console.WriteLine(h);
            Human ivan = TesHuman("Ivan");
            show(ivan);
            // show = StudentPrint; //error
            Student st = TesStudent("Inna");
            show(st);
            /////////////////////////////////////////////////////////
            ISetPiston<V4> setPiston2 = new Block<V4>();  //інваріантність
            ISetPiston<V4> setPiston = new Block<Engine>();  //Контраваріантність
            setPiston.PistonToEngine(new V4());
            //setPiston.PistonToEngine(new V8());
            // setPiston.PistonToEngine(new Engine());
        }
        static void TestInvar() //Інваріантність
        {
            object str = "Ivan";
            object i = 123;
            object d = 12.36;
            // List<object> list = new List<string>(); //Error
            List<string> list = new List<string>();
        }
        static void TestLinkedList()
        {
            Student Ivan = new Student("Ivan", new DateTime(2000, 10, 20), "IT Step");
            RandMarksStudent(Ivan);

            Student Piter = new Student("Piter", new DateTime(2012, 11, 15), "it Step");
            RandMarksStudent(Piter);

            Student Inna = new Student("Inna", new DateTime(2012, 11, 25), "ZHDTU");
            RandMarksStudent(Inna);

            LinkedList<Student> gr = new LinkedList<Student>();
            // LinkedListNode<Student>
            LinkedListNode<Student> Head = gr.AddFirst(Ivan);
            gr.AddAfter(Head, Piter);
            gr.AddLast(Inna);
            foreach (var st in gr)
            {
                Console.WriteLine(st);
            }
            Console.WriteLine(Head.Next.Next.Value);
            //LinkedListNode<Student> node = gr.Find(Inna);
            LinkedListNode<Student> node = gr.Find(new Student("13", DateTime.Today, "Step"));
            if (node != null)
                Console.WriteLine(node.Value);
            else
                Console.WriteLine("Not founded");
            //  gr.Sort() //Error
            //IEnumerable<Student> list= gr.OrderBy(s=>s);
            //foreach (var VARIABLE in list)
            //{
            //    Console.WriteLine(VARIABLE);
            // }
            //   gr.OrderBy(s => s).ToList().ForEach(s=> Console.WriteLine(s) );
            //  gr=new LinkedList<Student>(gr.OrderBy(s => s).ToList());
            gr = new LinkedList<Student>(gr.OrderBy(s => s));
            gr = new LinkedList<Student>(gr.OrderBy(s => s.Age));
            foreach (var VARIABLE in gr)
            {
                Console.WriteLine(VARIABLE);
            }



        }

        static void Main(string[] args)
        {
            //TestList();
            //TestGroup();
            //TestKo();
            // TestKontra();
            // TestInvar();
            TestLinkedList();
        }
    }
}