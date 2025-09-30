using Molyavchik.TaskPlanner.DataAccess.Abstractions;
using Molyavchik.TaskPlanner.DataAccess.FileStorage;
using Molyavchik.TaskPlanner.Domain.Logic;
using Molyavchik.TaskPlanner.Domain.Models;
using Molyavchik.TaskPlanner.Domain.Models.Enums;
using System;
using System.Globalization;

internal static class Program
{
    private static readonly IWorkItemRepository Repository = new FileWorkItemsRepository();
    private static readonly SimpleTaskPlanner Planner = new SimpleTaskPlanner(Repository);

    public static void Main(string[] args)
    {
        Console.WriteLine("Task Planner App");

        while (true)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("[A]dd work item");
            Console.WriteLine("[B]uild a plan");
            Console.WriteLine("[M]ark work item as completed");
            Console.WriteLine("[R]emove a work item");
            Console.WriteLine("[Q]uit");

            Console.Write("> ");
            var choice = Console.ReadLine()?.Trim().ToUpper();

            switch (choice)
            {
                case "A":
                    AddWorkItem();
                    break;
                case "B":
                    BuildPlan();
                    break;
                case "M":
                    MarkAsCompleted();
                    break;
                case "R":
                    RemoveWorkItem();
                    break;
                case "Q":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Unknown option. Try again.");
                    break;
            }
        }
    }

    private static void AddWorkItem()
    {
        Console.WriteLine("\nEnter a new work item:");

        Console.Write("Title: ");
        var title = Console.ReadLine();

        Console.Write("Due Date (dd.MM.yyyy): ");
        if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, DateTimeStyles.None, out var dueDate))
        {
            Console.WriteLine("Invalid Due Date format.");
            return;
        }

        Console.Write("Priority (None, Low, Medium, High, Urgent): ");
        if (!Enum.TryParse(Console.ReadLine(), true, out Priority priority))
        {
            Console.WriteLine("Invalid Priority value.");
            return;
        }

        Console.Write("Complexity (None, Minutes, Hours, Days, Weeks): ");
        if (!Enum.TryParse(Console.ReadLine(), true, out Complexity complexity))
        {
            Console.WriteLine("Invalid Complexity value.");
            return;
        }

        Console.Write("Description: ");
        var description = Console.ReadLine();

        Console.Write("Is it completed? (true/false): ");
        bool.TryParse(Console.ReadLine(), out bool isCompleted);

        var workItem = new WorkItem
        {
            Title = title,
            DueDate = dueDate,
            Priority = priority,
            Complexity = complexity,
            Description = description,
            IsCompleted = isCompleted,
            CreationDate = DateTime.Now
        };

        var id = Repository.Add(workItem);
        Repository.SaveChanges();
        Console.WriteLine($"Work item added with ID: {id}");
    }

    private static void BuildPlan()
    {
        var sortedItems = Planner.CreatePlan();

        if (sortedItems.Length == 0)
        {
            Console.WriteLine("No work items to plan.");
            return;
        }

        Console.WriteLine("\nSorted Work Items (Plan):");
        foreach (var item in sortedItems)
        {
            Console.WriteLine($"{item.Id} - {item.Title} (Due: {item.DueDate:dd.MM.yyyy}, Priority: {item.Priority})");
        }
    }

    private static void MarkAsCompleted()
    {
        var items = Repository.GetAll();
        if (items.Length == 0)
        {
            Console.WriteLine("No work items available.");
            return;
        }

        Console.WriteLine("\nEnter the ID of the work item to mark as completed:");
        foreach (var wi in items)
        {
            Console.WriteLine($"{wi.Id} - {wi.Title} (Completed: {wi.IsCompleted})");
        }

        if (Guid.TryParse(Console.ReadLine(), out var id))
        {
            var item = Repository.Get(id);
            if (item != null)
            {
                item.IsCompleted = true;
                Repository.Update(item);
                Repository.SaveChanges();
                Console.WriteLine("Work item marked as completed.");
            }
            else
            {
                Console.WriteLine("No work item found with this ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }

    private static void RemoveWorkItem()
    {
        var items = Repository.GetAll();
        if (items.Length == 0)
        {
            Console.WriteLine("No work items available.");
            return;
        }

        Console.WriteLine("\nEnter the ID of the work item to remove:");
        foreach (var wi in items)
        {
            Console.WriteLine($"{wi.Id} - {wi.Title}");
        }

        if (Guid.TryParse(Console.ReadLine(), out var id))
        {
            if (Repository.Remove(id))
            {
                Repository.SaveChanges();
                Console.WriteLine("Work item removed.");
            }
            else
            {
                Console.WriteLine("No work item found with this ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }
}
