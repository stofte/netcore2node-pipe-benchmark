namespace Client
{
    using System;
    using System.Collections.Generic;

    class TestData
    {
        public string LoremIpsum { get; set;}
        public int SomeWhatLongerFieldName;
        public Guid SystemId { get; set; }
        public DateTime TS { get; set; }

        public static IEnumerable<TestData> Generate(int count)
        {
            var list = new List<TestData>();
            for (var i = 0; i < count; i++)
            {
                yield return new TestData
                {
                    LoremIpsum = GetLorem(),
                    SomeWhatLongerFieldName = Rand.Next(0, Int32.MaxValue),
                    SystemId = Guid.NewGuid(),
                    TS = DateTime.Now.AddSeconds(Rand.Next(-100000, 0))
                };
            }
        }

        static Random Rand = new Random();

        static string GetLorem()
        {
            
            var start = Rand.Next(0, Lorem.Length - 2);
            var end = Rand.Next(start, Lorem.Length - 1);
            return Lorem.Substring(start, end - start);
        }

        static string Lorem = _rawlorem.Replace("\\r\\n", "");

        const string _rawlorem = @"
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur quis erat iaculis, rutrum purus blandit, vestibulum lacus. Phasellus suscipit rhoncus tempus. Nulla elit lectus, congue a purus at, elementum dapibus mauris. Quisque tellus massa, pretium porttitor vehicula quis, condimentum vel justo. Sed suscipit vel nunc quis faucibus. Sed nec blandit urna, vitae euismod justo. Sed fermentum malesuada mauris vitae accumsan.

Donec rhoncus, felis non dignissim laoreet, arcu eros pretium dolor, sit amet consectetur arcu urna vel mauris. Etiam suscipit ante metus, nec sodales lacus ullamcorper id. Etiam dapibus est viverra, finibus nunc vitae, accumsan tortor. Nullam sed pharetra nisi, eget convallis quam. Sed rhoncus et risus vitae lobortis. Maecenas a venenatis magna, in tristique tellus. Vestibulum luctus dictum fringilla. Quisque faucibus neque non eros congue accumsan. Suspendisse ut sapien mauris. Phasellus blandit sodales purus ut rhoncus. Quisque iaculis, ligula id feugiat eleifend, metus nisl lacinia ex, vitae placerat nisi elit non metus. Pellentesque nec nibh tristique, imperdiet nunc eget, ultricies ante. Fusce pretium laoreet iaculis. Curabitur sit amet eros ante.

Nullam non metus porta, condimentum purus vel, volutpat nulla. Nunc dolor dui, consectetur ac lacinia vitae, elementum vitae nulla. In non vestibulum nisi. Aenean ut diam finibus, varius diam ut, rhoncus metus. Proin justo ipsum, eleifend et scelerisque ac, egestas sed nibh. Nulla malesuada lorem quis placerat lacinia. Nunc in odio at risus ullamcorper consequat eu vitae velit. Curabitur at nulla euismod, aliquam dui a, commodo dui.

Sed consectetur venenatis mollis. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Nunc accumsan risus in accumsan vestibulum. Etiam suscipit ligula aliquet interdum euismod. Vestibulum condimentum blandit ante, ut accumsan sem pulvinar imperdiet. Quisque varius tincidunt augue maximus accumsan. Fusce consequat risus lacus, eget ullamcorper erat aliquet dapibus. Etiam posuere ante at mattis commodo.

Mauris est orci, commodo quis maximus vel, fermentum a dui. Suspendisse dignissim sodales mi at malesuada. Integer vitae turpis risus. Phasellus faucibus purus vitae tortor auctor porttitor. Quisque a felis mi. Vivamus rhoncus tincidunt gravida. Fusce quis eros ultrices, congue neque non, congue ligula. Proin sollicitudin ut justo id imperdiet. Pellentesque consectetur diam sem, ut suscipit erat laoreet nec. Suspendisse eget mauris lorem. Quisque feugiat lacinia nisl, rhoncus tincidunt augue varius vel. Nam neque purus, pulvinar ut mi a, cursus pellentesque neque. Vestibulum rhoncus dolor tortor, sed lobortis dui egestas eu. Fusce et neque consequat, pulvinar nisi eget, dignissim ex.

Sed et iaculis massa. Nulla finibus odio nec lectus malesuada, non posuere magna vulputate. Donec massa leo, tempor eget egestas ac, vestibulum quis lacus. Praesent quam nulla, tincidunt vel facilisis id, iaculis nec est. Fusce tristique semper tempus. Vivamus nec massa at diam faucibus finibus ut eget augue. Aliquam arcu arcu, convallis in mi in, volutpat venenatis diam. Mauris nisi magna, interdum id vulputate ut, semper sit amet quam.

Nullam eu sapien tempus ex varius placerat sit amet vel sapien. Vivamus imperdiet, dui quis vehicula dictum, enim mauris porta velit, nec ornare ipsum lectus ut neque. Donec hendrerit erat diam, in ultricies mauris iaculis id. Sed vehicula imperdiet lacinia. Maecenas id dapibus lectus. Suspendisse eget lacus tellus. Nam augue urna, aliquet in mi sit amet, suscipit congue est. Quisque euismod finibus quam eget dignissim. Etiam sit amet urna in lorem ultrices consectetur. Curabitur ut felis ante. Vivamus sagittis tortor ullamcorper mi commodo, nec pellentesque sapien luctus. Pellentesque nec diam pellentesque, facilisis augue non, varius dui.

In at pellentesque erat. Integer accumsan pretium sagittis. Pellentesque ac mi sit amet tellus faucibus aliquet. Quisque fermentum feugiat enim, in consectetur purus tincidunt id. Aenean neque mauris, sagittis in bibendum in, ultricies ac mi. Sed malesuada consequat elit, ac consectetur tortor cursus nec. Aliquam ante massa, mattis eu justo vel, cursus consequat lacus. Fusce efficitur risus eros, et maximus justo luctus ac. Ut tincidunt sit amet libero id luctus. Etiam non nisi dictum, vehicula libero et, fringilla ante. Suspendisse neque enim, scelerisque vitae feugiat tempus, bibendum finibus risus.

In hac habitasse platea dictumst. Nullam sed urna diam. Praesent tempus maximus efficitur. Fusce sodales at ante pellentesque finibus. Aliquam ullamcorper lacus vel leo ullamcorper rhoncus. Vivamus vitae tincidunt metus. Nullam volutpat, urna fermentum tempus dictum, lectus ante vulputate lorem, non varius orci ante non sem. Mauris tincidunt dictum felis, vitae auctor velit venenatis ut.

Cras fringilla luctus mauris, convallis aliquam justo ultricies nec. Vivamus ac vestibulum neque, ac tristique nisl. Nullam interdum ullamcorper nullam.        
";
    }
}
