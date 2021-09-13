using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BatchAddingParameters
{
    /// <summary>
    /// Логика взаимодействия для WindowHelp.xaml
    /// </summary>
    public partial class WindowHelp : Window
    {
        public WindowHelp()
        {
            InitializeComponent();
            Loaded += WindowHelp_Loaded;
        }

        private void WindowHelp_Loaded(object sender, RoutedEventArgs e)
        {
            textBox.Text = "\tПлагин позволяет добавить общий параметр в семейства без их открытия, " +
                "а так же удалить общий параметр, если он присутствует в них и есть такая возможность.\n" +
                "\tСлева выбирается параметр из Файла общих параметров, а справа можно выбрать путь, где расположены семейства. Если выбрать одно семейство" +
                ", то изменения коснутся только этого семейство, если выделить папку, то изменения коснуться всех семейств в папке, и если поставить галку" +
                " Подпапки, то так же изменения будут и в семействах во вложенных папках.\n" +
                "\tПри добавлении параметра в семейства так же можно выбрать группу параметров семейства, в которую нужно добавить параметр, и в " +
                "типоразмер или экземпляр семейства. При удалении параметра эти поля игнорируются.\n" +
                "\tВ текстовом поле внизу будет выведен отчет по всем изменениям в семействах.";
        }

        private void buttonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
