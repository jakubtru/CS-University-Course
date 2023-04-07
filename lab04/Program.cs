
/**
 * Celem laboratorium jest zapoznanie z zastosowaniem LINQ do obsługi danych w kolekcjach.
 * W celu realizacji laboratorium należy wczytać pliki zawierające dane zakodowane w formacie CSV a następnie dokonać na nich szeregu operacji.

1. [3 punkty] odwzoruj rekordy danych z plików regions.csv, territories.csv, employee_territories.csv, 
employees.csv przy pomocy odpowiednich klas. Dla uproszczenia uznaj, że każde pole jest typu String. 
Wczytaj wszystkie dane do czterech kolekcji typu List zawierających obiekty tych klas. 

2. [1 punkt] wybierz nazwiska wszystkich pracowników.

3. [1 punkt] wypisz nazwiska pracowników oraz dla każdego z nich nazwę regionu i terytorium gdzie pracuje. 
Rezultatem kwerendy LINQ będzie "płaska" lista, więc nazwiska mogą się powtarzać (ale każdy rekord będzie unikalny).

4. [1 punkt] wypisz nazwy regionów oraz nazwiska pracowników, którzy pracują w tych regionach, 
pracownicy mają być zagregowani po regionach, rezultatem ma być lista regionów z podlistą pracowników (odpowiednik groupjoin).

5. [1 punkt] wypisz nazwy regionów oraz liczbę pracowników w tych regionach.

6. [3 punkty] wczytaj do odpowiednich struktur dane z plików orders.csv oraz orders_details.csv. 
Następnie dla każdego pracownika wypisz liczbę dokonanych przez niego zamówień, średnią wartość zamówienia 
oraz maksymalną wartość zamówienia.
 */

namespace ConsoleApp5
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            //ZADANIE 1:
            
            Wczytywacz<Region> wczytywaczRegionow = new Wczytywacz<Region>();
            List<Region> regiony = wczytywaczRegionow.WczytajListe("regions.csv", Region.Create);
            /*foreach(Region region in regiony)
            {
                Console.WriteLine(region.RegionID + " " + region.RegionDescription);
            }*/

            Wczytywacz<Territory> wczytywaczTerytoriow = new Wczytywacz<Territory>();
            List<Territory> terytoria = wczytywaczTerytoriow.WczytajListe("territories.csv", Territory.Create);
            /*foreach (Territory territory in terytoria)
            {
                Console.WriteLine(territory.TerritoryID + " " + territory.TerritoryDescription + " " + territory.RegionID);
            }
            */
            Wczytywacz<Employee> wczytywaczPracownikow = new Wczytywacz<Employee>();
            List<Employee> pracownicy = wczytywaczPracownikow.WczytajListe("employees.csv", Employee.Create);
            /*foreach (Employee employee in pracownicy)
            {
                Console.WriteLine(employee.EmployeeID + " " + employee.LastName + " " + employee.FirstName + " " + employee.Title + " " + employee.TitleOfCourtesy + " " + employee.BirthDate + " " + employee.HireDate + " " + employee.Address + " " + employee.City + " " + employee.Region + " " + employee.PostalCode + " " + employee.Country + " " + employee.HomePhone + " " + employee.Extension + " " + employee.Photo + " " + employee.Notes + " " + employee.ReportsTo + " " + employee.PhotoPath);
            }
            */
            Wczytywacz<EmployeeTerritory> wczytywaczTerytoriowPracownikow = new Wczytywacz<EmployeeTerritory>();
            List<EmployeeTerritory> terytoriaPracownikow =
                wczytywaczTerytoriowPracownikow.WczytajListe("employee_territories.csv", EmployeeTerritory.Create);
            /*foreach (EmployeeTerritory employeeTerritory in terytoriaPracownikow)
            {
                Console.WriteLine(employeeTerritory.EmployeeID + " " + employeeTerritory.TerritoryID);
            }*/
            
            
            
            //ZADANIE 2:
            
            var zad2 = (from p in pracownicy select new { nazwisko = p.LastName });
            foreach (var n in zad2) 
            {
                Console.WriteLine(n.nazwisko);
            }
            
            
            //ZADANIE 3:
            
            var zad3 = (from p in pracownicy
                join t in terytoriaPracownikow on p.EmployeeID equals t.EmployeeID
                join tt in terytoria on t.TerritoryID equals tt.TerritoryID
                join r in regiony on tt.RegionID equals r.RegionID
                select new
                {
                    nazwisko = p.LastName, region = r.RegionDescription, terytorium = tt.TerritoryDescription
                });
            foreach (var p in zad3)
            {
                Console.WriteLine("nazwisko: " + p.nazwisko + " region: " + p.region + " terytorium: " + p.terytorium);
            }
            
            
            //ZADANIE 4:
            
            var zad4 = (from r in (from p in pracownicy
                    join t in terytoriaPracownikow on p.EmployeeID equals t.EmployeeID
                    join tt in terytoria on t.TerritoryID equals tt.TerritoryID
                    join r in regiony on tt.RegionID equals r.RegionID
                    select new { nazwisko = p.LastName, region = r.RegionDescription })
                group r by r.region
                into zgrupowane
                select (zgrupowane));
            foreach (var et in zad4)
            {
                Console.WriteLine("{0}:", et.Key);
                foreach (var pr in et.Distinct())
                {
                    Console.WriteLine("  {0}", pr.nazwisko);
                }
            }
            
            
            //ZADANIE 5:
            
            var zad5 = from region in regiony
                join territory in terytoria on region.RegionID equals territory.RegionID
                join employeeTerritory in terytoriaPracownikow on territory.TerritoryID equals employeeTerritory.TerritoryID
                join employee in pracownicy on employeeTerritory.EmployeeID equals employee.EmployeeID
                group employee by region.RegionDescription into g
                select new {region = g.Key, liczbaPracownikow = g.Distinct().Count()};
            foreach (var et in zad5)
            {
                Console.WriteLine(et.region + ": " + et.liczbaPracownikow);
            
            }
            

            //ZADANIE 6:
            
            
            Wczytywacz<Order> wczytywaczZamowien = new Wczytywacz<Order>();
            List<Order> zamowienia = wczytywaczZamowien.WczytajListe("orders.csv", Order.Create);
            Wczytywacz<OrderDetail> wczytywaczSzczegolowZamowien = new Wczytywacz<OrderDetail>();
            List<OrderDetail> szczegolyZamowien = wczytywaczSzczegolowZamowien.WczytajListe("orders_details.csv", OrderDetail.Create);

            var zad6 = from employee in pracownicy
                join order in zamowienia on employee.EmployeeID equals order.EmployeeID
                join orderDetail in szczegolyZamowien on order.OrderID equals orderDetail.OrderID
                group orderDetail by employee.LastName into g
                select new {nazwisko = g.Key, 
                    liczbaZamowien = g.Select(o=>o.OrderID).Distinct().Count(), 
                    sredniaWartoscZamowienia = g.Average(x => double.Parse(x.unitprice, System.Globalization.CultureInfo.InvariantCulture) 
                                                              * double.Parse(x.quantity, System.Globalization.CultureInfo.InvariantCulture) 
                                                              * (1 - double.Parse(x.discount, System.Globalization.CultureInfo.InvariantCulture))), 
                    maksymalnaWartoscZamowienia = g.Max(x => double.Parse(x.unitprice, System.Globalization.CultureInfo.InvariantCulture)
                                                             * double.Parse(x.quantity, System.Globalization.CultureInfo.InvariantCulture) 
                                                             * (1 - double.Parse(x.discount, System.Globalization.CultureInfo.InvariantCulture)))};

            foreach (var z in zad6)
            {
                Console.WriteLine("nazwisko: " + z.nazwisko + " | liczba zamowien: " + z.liczbaZamowien + " | srednia wartosc: " + Math.Round(z.sredniaWartoscZamowienia,2) + " | maksymalna: " + Math.Round(z.maksymalnaWartoscZamowienia,2));
                ;
            }
            

        }
    }
    
    public class Wczytywacz<T>
    {
        public List<T> WczytajListe(String path, Func<String[], T> generuj)
            {
                List<T> lista = new List<T>();
                String[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }
                    String[] data = lines[i].Split(',');
                    lista.Add(generuj(data));
                }
                return lista;
            }
    }
    
    public class Employee
{
    public String? EmployeeID {get; set;}
    public String? LastName {get; set;}
    public String? FirstName {get; set;}
    public String? Title {get; set;}
    public String? TitleOfCourtesy {get; set;}
    public String? BirthDate {get; set;}
    public String? HireDate {get; set;}
    public String? Address {get; set;}
    public String? City {get; set;}
    public String? Region {get; set;}
    public String? PostalCode {get; set;}
    public String? Country {get; set;}
    public String? HomePhone {get; set;}
    public String? Extension {get; set;}
    public String? Photo {get; set;}
    public String? Notes {get; set;}
    public String? ReportsTo {get; set;}
    public String? PhotoPath {get; set;}

    public static Employee Create(String[] data)
    {
        Employee employee = new Employee();
        employee.EmployeeID = data[0];
        employee.LastName = data[1];
        employee.FirstName = data[2];
        employee.Title = data[3];
        employee.TitleOfCourtesy = data[4];
        employee.BirthDate = data[5];
        employee.HireDate = data[6];
        employee.Address = data[7];
        employee.City = data[8];
        employee.Region = data[9];
        employee.PostalCode = data[10];
        employee.Country = data[11];
        employee.HomePhone = data[12];
        employee.Extension = data[13];
        employee.Photo = data[14];
        employee.Notes = data[15];
        employee.ReportsTo = data[16];
        employee.PhotoPath = data[17];
        return employee;
    }
}

public class Territory
{
    public String? TerritoryID {get; set;}
    public String? TerritoryDescription {get; set;}
    public String? RegionID {get; set;}

    public static Territory Create(String[] data)
    {
        Territory territory = new Territory();
        territory.TerritoryID = data[0];
        territory.TerritoryDescription = data[1];
        territory.RegionID = data[2];
        return territory;
    }
}

public class Region
{
    public String? RegionID {get; set;}
    public String? RegionDescription {get; set;}

    public static Region Create(String[] data)
    {
        Region region = new Region();
        region.RegionID = data[0];
        region.RegionDescription = data[1];
        return region;
    }
}

public class EmployeeTerritory
{
    public String? EmployeeID {get; set;}
    public String? TerritoryID {get; set;}

    public static EmployeeTerritory Create(String[] data)
    {
        EmployeeTerritory employeeTerritory = new EmployeeTerritory();
        employeeTerritory.EmployeeID = data[0];
        employeeTerritory.TerritoryID = data[1];
        return employeeTerritory;
    }
}
    
    public class OrderDetail
    {
        public string? OrderID, productid, unitprice, quantity, discount;

        public static OrderDetail Create(string[] data)
        {
            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = data[0];
            orderDetail.productid = data[1];
            orderDetail.unitprice = data[2];
            orderDetail.quantity = data[3];
            orderDetail.discount = data[4];
            return orderDetail;
        }
    }

    public class Order
    {
        public string? OrderID,
            customerid,
            EmployeeID,
            orderdate,
            requireddate,
            shippeddate,
            shipvia,
            freight,
            shipname,
            shipaddress,
            shipcity,
            shipregion,
            shippostalcode,
            shipcountry;

        public static Order Create(string[] data)
        {
            Order order = new Order();
            order.OrderID = data[0];
            order.customerid = data[1];
            order.EmployeeID = data[2];
            order.orderdate = data[3];
            order.requireddate = data[4];
            order.shippeddate = data[5];
            order.shipvia = data[6];
            order.freight = data[7];
            order.shipname = data[8];
            order.shipaddress = data[9];
            order.shipcity = data[10];
            order.shipregion = data[11];
            order.shippostalcode = data[12];
            order.shipcountry = data[13];
            return order;
        }
    }
}