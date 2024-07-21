dotnet tool install -g dotnet-script
dotnet-script HsmManager.csx <slot> <user_pin>



#r "nuget: Net.Pkcs11Interop"

using System;
using System.Collections.Generic;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;

void ShowAllObjects(Session session)
{
    List<ObjectAttribute> searchTemplate = new List<ObjectAttribute>();
    List<ObjectHandle> foundObjects = session.FindAllObjects(searchTemplate);

    if (foundObjects.Count == 0)
    {
        Console.WriteLine("No objects found.");
        return;
    }

    foreach (ObjectHandle obj in foundObjects)
    {
        List<ObjectAttribute> attributes = session.GetAttributeValue(obj, new List<CKA> { CKA.CKA_LABEL, CKA.CKA_ID });
        string label = attributes[0].GetValueAsString();
        byte[] id = attributes[1].GetValueAsByteArray();
        uint objectId = BitConverter.ToUInt32(id, 0);

        Console.WriteLine($"Object Label: {label}, Object ID: {objectId}");
    }
}

void DeleteObject(Session session, uint objectId)
{
    List<ObjectAttribute> searchTemplate = new List<ObjectAttribute>
    {
        new ObjectAttribute(CKA.CKA_ID, BitConverter.GetBytes(objectId))
    };

    List<ObjectHandle> foundObjects = session.FindAllObjects(searchTemplate);

    if (foundObjects.Count == 0)
    {
        Console.WriteLine("Object not found.");
        return;
    }

    foreach (ObjectHandle obj in foundObjects)
    {
        session.DestroyObject(obj);
        Console.WriteLine("Object deleted successfully.");
    }
}

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet-script HsmManager.csx <slot> <user_pin>");
    return;
}

int slotNumber = int.Parse(args[0]);
string userPin = args[1];

// Load the PKCS#11 library
string pkcs11LibraryPath = "path/to/your/pkcs11/library";
using (Pkcs11 pkcs11 = new Pkcs11(pkcs11LibraryPath, AppType.MultiThreaded))
{
    // Get the list of available slots
    List<Slot> slots = pkcs11.GetSlotList(SlotsType.WithTokenPresent);
    Slot slot = slots[slotNumber];

    // Open a read-write session
    using (Session session = slot.OpenSession(SessionType.ReadWrite))
    {
        // Login as user
        session.Login(CKU.CKU_USER, userPin);

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1: Show all objects' names and IDs");
            Console.WriteLine("2: Delete object using the object ID");
            Console.WriteLine("3: End");
            Console.Write("Choose an option (1, 2, or 3): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ShowAllObjects(session);
            }
            else if (choice == "2")
            {
                Console.Write("Enter the object ID to delete: ");
                string objectIdInput = Console.ReadLine();
                uint objectId;
                if (uint.TryParse(objectIdInput, out objectId))
                {
                    DeleteObject(session, objectId);
                }
                else
                {
                    Console.WriteLine("Invalid object ID.");
                }
            }
            else if (choice == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
            }
        }

        // Logout
        session.Logout();
    }
}
