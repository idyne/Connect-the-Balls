// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("EaEWpWl6HYiVScscULSdZs7N/WBkX0v6yXB+ONxMIejmQUwJ8cXMylHS3NPjUdLZ0VHS0tNdy8maQVkH+V8Dz7inQsrebnPHO6FFC+8089TL57V5wD4pUJrJ+yugAKkfoxBajqJLBEvfr4bmHDy3MWLHq22rlXUwXVK1zo2svbC/2qy8fu8yL7t6Of3jUdLx497V2vlVm1Uk3tLS0tbT0DO5enbaajh4NrScWMkyWD9F5tqhZFiuvEmj9aH4E7ZCsfDivVTWHorJ9RGIFYG7aJGjDYj1UYhyw3N0MeyVGmkLArsdVMRVXu8prAPXA3AzoKawX5vFrdFkgX1ZKhrnWcfHEJ4zzTRDgklAe2renYRC/Sd1EF90y7Wy20Iiw5EgeNHQ0tPS");
        private static int[] order = new int[] { 4,10,10,5,5,7,10,7,8,12,12,11,12,13,14 };
        private static int key = 211;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
