using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace win8mail.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : win8mail.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var inbox = new SampleDataGroup("Inbox",
                    "Inbox",
                    "You have 2 new messages out of 6",
                    "Assets/inbox.png",
                    "Hotmail, Facebook and Twitter messages");

            inbox.Items.Add(new SampleDataItem("inbox1",
                    "Heeeeeey!!!!!!",
                    string.Format("Sender:  Matt Hubbard, mattyh98712@hotmail.co.uk{0}Sent:     16 Nov 2012  21:34", Environment.NewLine),
                    "Assets/profile-matt.jpg",
                    "Whassup SDog, how's hanging?  I'm out this weekend partying hard, wanna come join?",
                    "Whassup SDog, how's hanging?  I'm out this weekend partying hard, wanna come join?",
                    inbox));
            inbox.Items.Add(new SampleDataItem("inbox2",
                    "Would you like to visit Alton Towers this summer?",
                    string.Format("Sender:  Richard Yoga, richy989823@hotmail.co.uk{0}Sent:     16 Nov 2012  21:30", Environment.NewLine),
                    "Assets/profile-rich.jpg",
                    "We are visiting the theme park in the summer would you like to come with us?",
                    "We are visiting the theme park in the summer would you like to come with us?",
                    inbox));
            inbox.Items.Add(new SampleDataItem("inbox3",
                    "I'm bored, fancy a Skype?",
                    string.Format("Sender:  Clare Kop, claire98298@hotmail.co.uk{0}Sent:     16 Nov 2012  20:30", Environment.NewLine),
                    "Assets/profile-clare.jpg",
                    "Skype!  Skype!!  Skype!!!",
                    "Skype!  Skype!!  Skype!!!",
                    inbox));
            inbox.Items.Add(new SampleDataItem("inbox4",
                    "Thanks for the birthday card",
                    string.Format("Sender:  Kate Poll, Kate8298@hotmail.co.uk{0}Sent:     16 Nov 2012  20:22", Environment.NewLine),
                    "Assets/profile-kate.jpg",
                    "I had a brill time for my birthday, glad you could come to the party - see you soon! X",
                    "I had a brill time for my birthday, glad you could come to the party - see you soon! X",
                    inbox));
            inbox.Items.Add(new SampleDataItem("inbox5",
                    "Skiing",
                    string.Format("Sender:  Meg Moo, Meg98@hotmail.co.uk{0}Sent:     16 Nov 2012  20:18", Environment.NewLine),
                    "Assets/profile-meg.jpg",
                    "I cannot wait to go skiing this winter in the Alps, are you looking forward to it too?",
                    "I cannot wait to go skiing this winter in the Alps, are you looking forward to it too?",
                    inbox));
            inbox.Items.Add(new SampleDataItem("inbox6",
                    "Beers",
                    string.Format("Sender:  Tommy Tee, tt@hotmail.co.uk{0}Sent:     16 Nov 2012  20:15", Environment.NewLine),
                    "Assets/profile-tom.jpg",
                    "Beers, tomorrow night, be there!",
                    "Beers, tomorrow night, be there!",
                    inbox));
            
            this.AllGroups.Add(inbox);

            var junk = new SampleDataGroup("Junk",
                "Junk",
                "You have 0 junk mail",
                "Assets/junk.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            this.AllGroups.Add(junk);

            var sent = new SampleDataGroup("SentItems",
                    "Sent Items",
                    "You have 87 sent items",
                    "Assets/sent.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            sent.Items.Add(new SampleDataItem("SentItems1",
                    "I'll be there",
                    string.Format("To:  Tommy Tee, tt@hotmail.co.uk{0}Sent:     16 Nov 2012  20:16", Environment.NewLine),
                    "Assets/profile-tom.jpg",
                    "Yes beer monster, I'll be there - see you then!",
                    "Yes beer monster, I'll be there - see you then!",
                    sent));
            sent.Items.Add(new SampleDataItem("SentItems2",
                    "Weekend plans",
                    string.Format("Sender:  Matt Hubbard, mattyh98712@hotmail.co.uk{0}Sent:     16 Nov 2012  20:34", Environment.NewLine),
                    "Assets/profile-matt.jpg",
                    "How's it hangin' MDog, any weekend plans?",
                    "How's it hangin' MDog, any weekend plans?",
                    sent));
           
            this.AllGroups.Add(sent);
        }
    }
}
