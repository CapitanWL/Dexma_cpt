string currentPath = @"C:\Users\capit\Desktop\CW\Dexma_cpt\Dexma_cpt_ServerSide\bin\Debug\net7.0\";
string targetPath = @"C:\Users\capit\Desktop\CW\Dexma_cpt\Dexma_cpt_ServerSide\Encryption\HubKeys\hub_public_key.json";

string relativePath = GetRelativePath(currentPath, targetPath);

Console.WriteLine(relativePath);


static string GetRelativePath(string fromPath, string toPath)
{
    Uri fromUri = new Uri(fromPath);
    Uri toUri = new Uri(toPath);

    Uri relativeUri = fromUri.MakeRelativeUri(toUri);

    return Uri.UnescapeDataString(relativeUri.ToString());
}

/*
using (DexmaDbContext db = new())
{using (DexmaDbContext db = new())
{
    await db.AddRelationType("Default");
    await db.AddRelationType("Block");
    await db.AddRelationType("Friend");
}

    await db.AddUser("capitan_wl", "79003452311", "Capitan", true, "ihatecodding");
    await db.AddUser("john_doe", "79015678345", "John Doe", true, "ihatecoding");
    await db.AddUser("alice_smith", "79123456789", "Alice Smith", true, "ihatecoding");
    await db.AddUser("max_power", "79987654321", "Max Power", true, "ihatecoding");
    await db.AddUser("emily_green", "79111111111", "Emily Green", true, "ihatecoding");
    await db.AddUser("alex_blue", "79022222222", "Alex Blue", true, "ihatecoding");
    await db.AddUser("luke_black", "79044444444", "Luke Black", true, "ihatecoding");
    await db.AddUser("mike_red", "79066666666", "Mike Red", true, "ihatecoding");
    await db.AddUser("david_purple", "79088888888", "David Purple", true, "ihatecoding");
    await db.AddUser("eva_orange", "79099999999", "Eva Orange", true, "ihatecoding");
    await db.AddUser("sophia_violet", "79234567890", "Sophia Violet", true, "ihatecoding");

    await db.AddRelationType("Default");
    await db.AddRelationType("Block");
    await db.AddRelationType("Friend");

    await db.AddRelation(1, 2, 3);
    await db.AddRelation(2, 1, 4);
    await db.AddRelation(2, 1, 5);
    await db.AddRelation(1, 5, 6);
    await db.AddRelation(1, 6, 1);
    await db.AddRelation(1, 7, 8);
    await db.AddRelation(1, 8, 9);
    await db.AddRelation(2, 9, 10);
    await db.AddRelation(1, 10, 11);
    await db.AddRelation(1, 1, 11);
    await db.AddRelation(1, 2, 1);
    await db.AddRelation(1, 3, 2);
    await db.AddRelation(2, 1, 3);
    await db.AddRelation(2, 5, 4);
    await db.AddRelation(1, 6, 5);
    await db.AddRelation(1, 7, 1);

    await db.AddMessage(1, "Message 1", DateTime.Now);
    await db.AddMessage(2, "Message 2", DateTime.Now.AddDays(-1));
    await db.AddMessage(3, "Message 3",  DateTime.Now.AddHours(-2));
    await db.AddMessage(4, "Message 4",  DateTime.Now.AddMinutes(-30));
    await db.AddMessage(5, "Message 5",  DateTime.Now.AddSeconds(-10));
    await db.AddMessage(6, "Message 6",  DateTime.Now.AddHours(-1));
    await db.AddMessage(7, "Message 7",  DateTime.Now.AddMinutes(-15));
    await db.AddMessage(8, "Message 8", DateTime.Now.AddSeconds(-5));
    await db.AddMessage(9, "Message 9", DateTime.Now.AddHours(-3));
    await db.AddMessage(10, "Message 10", DateTime.Now.AddMinutes(-20));
    await db.AddMessage(11, "Message 11", DateTime.Now.AddSeconds(-15));
    await db.AddMessage(12, "Message 12", DateTime.Now.AddHours(-2));
    await db.AddMessage(13, "Message 13", DateTime.Now.AddMinutes(-25));
    await db.AddMessage(14, "Message 14", DateTime.Now.AddSeconds(-8));
    await db.AddMessage(15, "Message 15", DateTime.Now.AddHours(-4));
    await db.AddMessage(16, "Message 16", DateTime.Now.AddMinutes(-10));
}

*/
