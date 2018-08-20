namespace ComputedMath.Models {
    public class BoxModel {
        public BoxModel() : this("", "") { }

        public BoxModel(string title, string content) {
            Title = title;
            Content = content;
        }

        public string Title { get; set; }
        public string Content { get; set; }
    }
}