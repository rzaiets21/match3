// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("LK+hrp4sr6SsLK+vrjfXZ3x2/aE6bKVxZTk9GHk8KJi+yO5reKKV9NucRBgymZX8GHgTskAj6uOhSdiduORi3npbUoXbenPcNMH8XFnYXI+eLK+MnqOop4Qo5ihZo6+vr6uureuCAW9XYJcVCFPXnV6c3etSqUcXHTrbEE3lkE84gVRLgCh811QdEmxS+6A/+TyfORY7Da9IW0ZPHdBYR/ggFovsW+fiS8HUlLDPdFscvKVRJYucUXDGbB/61WJ3uDED0Cyk5UJeNIAPeDImtnaHiabppLr3UKKLN/n9Lfdhu5oUJ8nr6qqUyO9bY40SD0vKheYH2k3adu73DaENnqtdVRha+iTi2Cj8JnXsAd3+WZsC4/DQYb/KSbKO2VK0R6ytr66v");
        private static int[] order = new int[] { 1,8,10,11,8,11,13,12,10,10,10,11,12,13,14 };
        private static int key = 174;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
