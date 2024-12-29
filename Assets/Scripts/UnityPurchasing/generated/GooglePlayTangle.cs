// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("t10dkzF84bJ5J3Zs265ImccXU1dJ+3hbSXR/cFP/Mf+OdHh4eHx5evt4dnlJ+3hze/t4eHnFFqFMbRpEm9FyVF17U28+Qkk4oroa8hz4muiKBNb2S/xzoq5R7MiI1fbeKCfbdYNypCrdbT3OgGaVXDF3EMc2THe7FvptJubu8xSn3JSx2R/AbpHYpaXRXxIxY5dPtWWNWLTvu+IvSF0SGZkQqZgP6HLZRqeG069r7hAzw7Ft7TyO5kjOOjwJXk62TVlxBW/Gv5WHlBQk+2B7q9/sTWYJoEiwezPQ5RBVfupwX/klr1V+GFg1+iwdBDIeeShHVg8K+dDUSF4o0Mdi8QMGXnT9QBoLc7fVIDHLsJW+oqGUsVanudTD89ftQCV8cHt6eHl4");
        private static int[] order = new int[] { 9,9,9,6,6,11,7,11,8,10,10,13,13,13,14 };
        private static int key = 121;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
