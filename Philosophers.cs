using System;
using System.Threading;

namespace philosophers_os
{
    class Fork
    {
        public bool free = true;
        public int number;
        public void take()
        {
            this.free = false;
        }

        public void put()
        {
            this.free = true;
        }
    };

    class Philosopher
    {
        int id;
        Fork fork_left;
        Fork fork_right;
        uint eat_count;
        double wait_time;
        DateTime wait_start;
        bool stop_flag;
        bool debug_flag;
        Random random;

        void think()
        {
            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " thinking");
            }

            Thread.Sleep(this.random.Next(0, 100));

            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " hungry");
            }

            this.wait_start = DateTime.Now;
        }

        void eat()
        {
            this.wait_time += DateTime.Now.Subtract(this.wait_start).TotalMilliseconds;
            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " eating");
            }

            Thread.Sleep(this.random.Next(0, 100));

            eat_count++;
        }

        public Philosopher(int number, Fork left, Fork right, bool dbg)
        {
            this.id = number;
            this.fork_left = left;
            this.fork_right = right;
            this.eat_count = 0;
            this.wait_time = 0;
            this.debug_flag = dbg;
            this.stop_flag = false;
            this.random = new Random();
        }

        public void takeLeft()
        {
            this.fork_left.take();
            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " took left fork");
            }
        }

        public void takeRight()
        {
            this.fork_right.take();
            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " took right fork");
            }
        }

        public void putLeft()
        {
            this.fork_left.put();
            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " put left fork");
            }
        }

        public void putRight()
        {
            this.fork_right.put();
            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " put right fork");
            }
        }

        public void run()
        {
            var leftNumber = this.fork_left.number;
            var rightNumer = this.fork_right.number;
            while (!stop_flag)
            {
                think();
                while (!(this.fork_left.free && this.fork_right.free))
                {
                    Thread.Sleep(1);
                }
                if (leftNumber > rightNumer)
                {
                    takeLeft();
                    takeRight();
                }
                else
                {
                    takeRight();
                    takeLeft();
                }
                eat();
                if (leftNumber > rightNumer)
                {
                    putRight();
                    putLeft();
                }
                else
                {
                    putLeft();
                    putRight();
                }
            }
        }

        public void stop()
        {
            stop_flag = true;
        }

        public void printStats()
        {
            Console.WriteLine(this.id + " " + this.eat_count + " " + Convert.ToInt32(this.wait_time));
        }
    };

    class Program
    {
        static void Main(string[] args)
        {
            int N = 5;
            bool dbg = false;
            int duration = 60000;

            Fork[] forks = new Fork[N];
            for (int i = 0; i < N; i++)
            {
                forks[i] = new Fork() { number = i + 1 };
            }

            Philosopher[] phils = new Philosopher[N];
            for (int i = 0; i < N; i++)
            {
                phils[i] = new Philosopher(i + 1, forks[(i + 1) % N], forks[i], dbg);
            }

            Thread[] runners = new Thread[N];
            for (int i = 0; i < N; i++)
            {
                runners[i] = new Thread(phils[i].run);
            }
            for (int i = 0; i < N; i++)
            {
                runners[i].Start();
            }

            Thread.Sleep(duration);

            for (int i = 0; i < N; i++)
            {
                phils[i].stop();
            }

            for (int i = 0; i < N; i++)
            {
                runners[i].Join();
            }

            for (int i = 0; i < N; i++)
            {
                phils[i].printStats();
            }
        }
    }
}
