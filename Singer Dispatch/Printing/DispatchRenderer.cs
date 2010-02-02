using System.Text;

namespace SingerDispatch.Printing
{
    class DispatchRenderer : Renderer
    {
        public string GenerateHTML(object dispatch)
        {
            return GenerateHTML((Dispatch)dispatch);
        }

        public string GenerateHTML(Dispatch dispatch)
        {            
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Singer Specialized - Dispatch"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(GetHeader("9244-01-01"));
            content.Append(GetDetails());
            content.Append(GetDescription("Supply mean and equipment to transport 1450 HP compressor package skid - Winter weight restriction"));
            content.Append(GetEquipment());
            content.Append(GetSchedule());
            content.Append(GetLoadInstructions());
            content.Append(GetDimensions());
            content.Append(GetTractors());
            content.Append(GetSingerPilots());
            content.Append(GetThirdPartyPilots());
            content.Append(GetThridPartyServices());
            content.Append(GetWireLiftInfo());
            content.Append(GetPermits());
            content.Append(GetOtherInfo());
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private string GetStyles()
        {
            string content = @"
                <style type=""text/css"">
                    /***** RESET DEFAULT BROWSER STYLES *****/
                    html, body, div, span, applet, object, iframe,
                    h1, h2, h3, h4, h5, h6, p, blockquote, pre,
                    a, abbr, acronym, address, big, cite, code,
                    del, dfn, em, font, img, ins, kbd, q, s, samp,
                    small, strike, strong, sub, sup, tt, var,
                    b, u, i, center,
                    dl, dt, dd, ol, ul, li,
                    fieldset, form, label, legend,
                    table, caption, tbody, tfoot, thead, tr, th, td 
                    {
                        margin: 0;
                        padding: 0;
                        border: 0;
                        outline: 0;
                        font-size: 100%;
                        vertical-align: baseline;
                        background: transparent;
                    }
                    body 
                    {
                        line-height: 1.2em;
                        padding: 5px;
                    }
                    ol, ul 
                    {
                        list-style: none;
                    }
                    blockquote, q 
                    {
                        quotes: none;
                    }
                    blockquote:before, blockquote:after,
                    q:before, q:after 
                    {
                        content: '';
                        content: none;
                    }

                    /* remember to define focus styles! */
                    :focus 
                    {
                        outline: 0;
                    }

                    /* remember to highlight inserts somehow! */
                    ins
                    {
                        text-decoration: none;
                    }
                    del 
                    {
                        text-decoration: line-through;
                    }
                    table
                    {             
                    }
                    td
                    {
                    }
                    
                    
                    /*******/
                    
                    body
                    {
                        font-size: 11px;
                        font-family: Arial, Helvetica, Tahoma, sans-serif;
                    }

                    th
                    {
                        text-align: left;
                    }

                    span.field_name
                    {
                        font-weight: bold;
                    }

                    div#header
                    {
                        
                    }
                    
                    div#header table 
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    div#header td
                    {                
                        vertical-align: top;
                        padding: 10px;
                    }
                    
                    div#header td#logo_col
                    {
                        width: 200px;
                        
                    }
                    
                    div#header td#address_col
                    {
                        
                    }
                    
                    div#header td#id_col
                    {             
                        text-align: center;
                        font-weight: bold;
                    }
                    
                    div#header span
                    {
                        display: block;
                    }
                    
                    div#header span.title
                    {
                        display: block;
                        font-weight: bold;
                        font-size: 1.5em;
                        padding: 0.5em 0.3em;
                        text-align: center;
                        border-top: dotted 1px #000000;
                        border-bottom: dotted 1px #000000;
                    }
                    
                    div#details
                    {
                        padding: 10px;                
                    }            
                    
                    div#details table#dispatch_info, div#details table#departure_info
                    {
                        margin-bottom: 10px;
                    }
                    
                    div#details td.field_name
                    {
                        font-weight: bold;
                        white-space: nowrap;
                        padding-bottom: 5px;
                        padding-right: 10px;
                    }
                    
                    div#details td.value
                    {
                        padding-right: 15px;
                    }
                    
                    div.section
                    {
                        padding: 10px;
                        margin-top: 2px;
                        border-top: dotted 1px #000000;
                    }
                    
                    div.section span.heading
                    {
                        font-weight: bold;
                        display: block;                
                        margin-bottom: 10px;
                    }

                    div#load_and_unload span.heading
                    {
                        
                    }

                    div#load_and_unload hr
                    {
                        border: none;
                        border-top: dotted 1px #000000;
                        margin: 10px 0;
                    }

                    div#load_and_unload div.commodity
                    {
                    }

                    div#load_and_unload span.commodity_name
                    {
                        font-weight: bold;                
                    }

                    div#load_and_unload div.loading, div#load_and_unload div.unloading
                    {
                        margin-top: 10px;                
                    }

                    div#load_and_unload div.loading span.heading, div#load_and_unload div.unloading span.heading
                    {
                        text-decoration: underline;
                    }

                    div#load_and_unload td
                    {
                        padding-right: 10px;
                    }

                    div#load_and_unload table.instructions
                    {
                        margin-top: 10px;
                        width: 100%;
                    }

                    div#load_and_unload table.instructions td
                    {
                        width: 33%;
                    }

                    div#dimensions table.dimensions
                    {
                        width: 100%;
                        margin-bottom: 5px;
                    }

                    div#dimensions table.weights
                    {
                        width: 100%
                    }

                    div#dimensions table.weights th
                    {
                        text-align: center;
                    }

                    div#dimensions table.weights th.vertical
                    {
                        text-align: left;
                    }
                    
                    div#dimensions table.weights td
                    {
                        text-align: center;
                     
                    }

                    div#tractors td, div#other_equipment td
                    {
                        padding-right: 10px;
                        padding-bottom: 2px;
                    }

                    div#third_party_pilot table,
                    div#thid_party_services table,
                    div#wire_lifts table,
                    div#permits table
                    {
                        width: 100%;
                    }

                    div#third_party_pilot th, 
                    div#thid_party_services th,
                    div#wire_lifts th,
                    div#permits th
                    {
                        padding-bottom: 10px;
                    }

                    div#third_party_pilot tr.comments td, 
                    div#thid_party_services tr.comments td,
                    div#wire_lifts tr.comments td,
                    div#permits tr.comments td
                    {
                        padding: 5px 15px;
                        padding-bottom: 10px;                
                    }
                </style>
            ";

            return content;
        }

        private string GetHeader(string dispatchNumber)
        {
            string content = @"
                <div id=""header"">
                    <table>
                        <tr>
                            <td id=""logo_col"">
                                <span class=""logo""><img src=""\\sindcx001\Storage\Programers\logo.png"" alt=""Singer Specialized""></span>
                            </td>
                            <td id=""address_col"">
                                <span>Singer Specialized Ltd.</span>
                                <span>Site 12 Box 26 RR5</span>
                                <span>Calgary, AB T2P 2G6</span>
                                <span>Phone: (403) 569 - 8605</span>
                            </td>
                            <td id=""id_col"">
                                <span>File Copy</span>
                                <span>%DISPATCH_NUMBER%</span>
                            </td>
                        </tr>
                    </table>
                    
                    <span class=""title"">Dispatch Order</span>            
                </div>
            ";

            return content.Replace("%DISPATCH_NUMBER%", dispatchNumber);
        }

        private string GetDetails()
        {
            string content = @"
                <div id=""details"">
                    <table id=""dispatch_info"">
                        <tr>
                            <td class=""field_name col1_4"">Date:</td>
                            <td class=""value col2_4"">January 6, 2009</td>
                            <td class=""field_name col3_4"">Customer #:</td>
                            <td class=""value col4_4"">Harmattan Gas Processing Partnership C/O Excelsior Engineering</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Unit #:</td>
                            <td class=""value"">03-12</td>
                            <td class=""field_name"">Trailer #:</td>
                            <td class=""value"">64Wheel - 35-81,35-67-29-67</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Driver:</td>
                            <td class=""value"">John Hall</td>
                            <td class=""field_name"">Swampers:</td>
                            <td class=""value""></td>
                        </tr>
                    </table>
                    
                    <table id=""departure_info"">
                        <tr>
                            <td class=""field_name col1_2"">Depart Date:</td>
                            <td class=""value col2_2"">Jan 7, 09 - 09:00</td>
                        </tr>                
                        <tr>                    
                            <td class=""field_name"">Depart From:</td>
                            <td class=""value"">Propak Main Yard</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Depart Units:</td>
                            <td class=""value"">#12,9909,55,47,18,42</td>
                        </tr>
                    </table>
                   
                    <table id=""customer_references"">
                        <tr>
                            <td class=""field_name col1_2"">Customer References:</td>
                            <td class=""value col2_2""><span class=""reference""><span class=""field_name"">AFE</span>: <span class=""value"">9342</span></span>, <span class=""reference""><span class=""field_name"">PO#</span>: <span class=""value"">13940</span></span></td>
                        </tr>
                    </table>
                </div>
            ";

            return content;
        }

        private string GetDescription(string description)
        {
            string content = @"
                <div id=""description"" class=""section"">
                    <span class=""heading"">Dispatch Description</span>
                    
                    <p>%DESCRIPTION%</p>
                </div>
            ";

            return content.Replace("%DESCRIPTION%", description);
        }

        private string GetEquipment()
        {
            string content = @"
                <div id=""equipment_requirements"" class=""section"">
                    <span class=""heading"">Equipment Required Information</span>
                    
                </div>
            ";

            return content;
        }

        private string GetSchedule()
        {
            string content = @"
                <div id=""schuedule"" class=""section"">
                    <span class=""heading"">Dispatch Schedule</span>
                    
                    <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce quis est sapien. Donec tempor, tortor at auctor scelerisque, justo elit condimentum enim, ac tristique nunc risus eu neque. Nam scelerisque nulla vel sapien facilisis ut commodo velit feugiat. Phasellus non mi ullamcorper neque porttitor semper at at dolor. Nullam ut erat at ipsum molestie tempus in nec lectus. Morbi risus ipsum, consectetur id laoreet ut, ullamcorper id urna. Duis a iaculis quam. Donec tempor, arcu eu cursus egestas, purus purus mattis velit, in suscipit neque metus nec augue. Nunc id justo vitae massa porta placerat sed vitae tellus. Aliquam erat.</p>
                </div>
            ";

            return content;
        }

        private string GetLoadInstructions()
        {
            string content = @"
                <div id=""load_and_unload"" class=""section"">
                    <span class=""heading"">Load/Unload Information</span>
                    
                    <div class=""commodity"">
                        <span class=""commodity_name"">1450 HP Compressor Package Skid</span>
                        
                        <div class=""dimensions"">
                            <span class=""weight"">74843kg</span>
                            <span class=""size"">11.8m x 7.24m x 6.25m (L x W x H)</span>                    
                        </div>
                        
                        <div class=""loading"">
                            <span class=""heading"">Load Information</span>
                            
                            <table class=""details"">
                                <thead>
                                    <tr>
                                        <th>Date</th>
                                        <th>Time</th>
                                        <th>Load Location</th>
                                        <th>Load Site Contact</th>
                                        <th>Load By</th>
                                        <th>Loading Contact</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>Jan 07, 2009</td>
                                        <td>10:00</td>
                                        <td>Propak Systems - 404 East Lake Rd, Airdrie</td>
                                        <td>Rob Ogle (403) 333 - 5369</td>
                                        <td>Singer Specialized</td>
                                        <td>Jordy Cropley (403) 816 - 1645</td>
                                    </tr>
                                </tbody>                        
                            </table>
                            
                            <table class=""instructions"">
                                <thead>
                                    <tr>
                                        <th>Load Route</th>
                                        <th>Load Instruction</th>
                                        <th>Load Placement/Orientation</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>84st-16ave-Deerfoot-2-567 east-right onto Eastlake rd to Propak</td>
                                        <td>Travel to Propack and load compressor by J&amp;R and load cooler with pickers</td>
                                        <td>Load compressor evenly between necks</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        
                        <div class=""unloading"">
                            <span class=""heading"">Unload Information</span>
                            
                            <table class=""details"">
                                <thead>
                                    <tr>
                                        <th>Date</th>
                                        <th>Time</th>
                                        <th>Load Location</th>
                                        <th>Load Site Contact</th>
                                        <th>Load By</th>
                                        <th>Loading Contact</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>Jan 7, 2009</td>
                                        <td>14:00</td>
                                        <td>Harmattan Gas Processiong Parnership - Harmatton-9-27-31-4-W4M</td>
                                        <td>Gord Fox (403) 335 - 7528</td>
                                        <td>Singer Specialized</td>
                                        <td>Jordy Cropley (403) 816 - 1645</td>
                                    </tr>
                                </tbody>                        
                            </table>
                            
                            <table class=""instructions"">
                                <thead>
                                    <tr>
                                        <th>Unload Route</th>
                                        <th>Unload Instruction</th>
                                        <th>Unload Placement/Orientation</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>Eastlake Rd north-567 west-772 south-567 west-22 north-Twp320(Bergen Rd) eas-RR402 south to site-Enter thru the Pipe yard</td>
                                        <td>Deliver both loads to Harmatton site in convoy and offload as directed</td>
                                        <td>See Gord Fox onsite for direction</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>

                    <hr>

                    <div class=""commodity"">
                        <span class=""commodity_name"">1450 HP Compressor Package Skid</span>

                        <div class=""dimensions"">
                            <span class=""weight"">74843kg</span>
                            <span class=""size"">11.8m x 7.24m x 6.25m (L x W x H)</span>
                        </div>

                        <div class=""loading"">
                            <span class=""heading"">Load Information</span>

                            <table class=""details"">
                                <thead>
                                    <tr>
                                        <th>Date</th>
                                        <th>Time</th>
                                        <th>Load Location</th>
                                        <th>Load Site Contact</th>
                                        <th>Load By</th>
                                        <th>Loading Contact</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>Jan 7, 2009</td>
                                        <td>14:00</td>
                                        <td>Harmattan Gas Processiong Parnership - Harmatton-9-27-31-4-W4M</td>
                                        <td>Gord Fox (403) 335 - 7528</td>
                                        <td>Singer Specialized</td>
                                        <td>Jordy Cropley (403) 816 - 1645</td>
                                    </tr>
                                </tbody>
                            </table>

                            <table class=""instructions"">
                                <thead>
                                    <tr>
                                        <th>Unload Route</th>
                                        <th>Unload Instruction</th>
                                        <th>Unload Placement/Orientation</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>Eastlake Rd north-567 west-772 south-567 west-22 north-Twp320(Bergen Rd) eas-RR402 south to site-Enter thru the Pipe yard</td>
                                        <td>Deliver both loads to Harmatton site in convoy and offload as directed</td>
                                        <td>See Gord Fox onsite for direction</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                        <div class=""unloading"">
                            <span class=""heading"">Unload Information</span>

                            <table class=""details"">
                                <thead>
                                    <tr>
                                        <th>Date</th>
                                        <th>Time</th>
                                        <th>Load Location</th>
                                        <th>Load Site Contact</th>
                                        <th>Load By</th>
                                        <th>Loading Contact</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>Jan 07, 2009</td>
                                        <td>10:00</td>
                                        <td>Propak Systems - 404 East Lake Rd, Airdrie</td>
                                        <td>Rob Ogle (403) 333 - 5369</td>
                                        <td>Singer Specialized</td>
                                        <td>Jordy Cropley (403) 816 - 1645</td>
                                    </tr>
                                </tbody>
                            </table>

                            <table class=""instructions"">
                                <thead>
                                    <tr>
                                        <th>Load Route</th>
                                        <th>Load Instruction</th>
                                        <th>Load Placement/Orientation</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>84st-16ave-Deerfoot-2-567 east-right onto Eastlake rd to Propak</td>
                                        <td>Travel to Propack and load compressor by J&amp;R and load cooler with pickers</td>
                                        <td>Load compressor evenly between necks</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            ";

            return content;
        }

        private string GetDimensions()
        {
            string content = @"
                <div id=""dimensions"" class=""section"">
                    <span class=""heading"">Dimensional Information</span>
                    
                    <table class=""dimensions"">
                        <tr>
                            <td><span class=""field_name"">Loaded Length</span>: <span class=""value"">40.00</span></td>
                            <td><span class=""field_name"">Loaded Width</span>: <span class=""value"">7.16</span></td>                    
                            <td><span class=""field_name"">Loaded Height</span>: <span class=""value"">6.82</span></td>
                            <td><span class=""field_name"">Gross Weight</span>: <span class=""value"">128,393</span></td>
                        </tr>
                    </table>
                    
                    <table class=""weights"">
                        <tr>
                            <th></th>
                            <th>Steer</th>
                            <th>Drive</th>
                            <th>Group 1</th>
                            <th>Group 3</th>
                            <th>Group 3</th>
                            <th>Group 4</th>
                            <th>Group 5</th>
                            <th>Group 6</th>
                            <th>Group 7</th>
                            <th>Group 8</th>
                            <th>Group 9</th>
                            <th>Group 10</th>                    
                        </tr>
                        <tr>
                            <th class=""vertical"">Estimated Axle Weight</th>
                            <td>7300</td>
                            <td>21000</td>
                            <td>28000</td>
                            <td>28000</td>
                            <td>28000</td>
                            <td>28000</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                        </tr>
                        <tr>
                            <th class=""vertical"">Scaled Axle Weight</th>
                            <td>7300</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                            <td>-</td>
                        </tr>
                    </table>
                </div>
            ";

            return content;
        }

        private string GetTractors()
        {
            string content = @"
                <div id=""tractors"" class=""section"">
                    <span class=""heading"">Tractors (Singer Service)</span>
                    
                    <table>
                        <thead>
                            <tr>
                                <th>Unit</th>
                                <th>Contact</th>
                                <th>Trailer Combination</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>03-27</td>
                                <td>Chad Congdon (403) 863-7209</td>
                                <td>18-76</td>
                            </tr>
                            <tr>
                                <td>03-22</td>
                                <td>Corey Cuthill (403) 999-5859</td>
                                <td>26-77</td>
                            </tr>
                            <tr>
                                <td>03-08</td>
                                <td>Chris Beutler (403) 852-9726</td>
                                <td>26-77</td>
                            </tr>
                            <tr>                    
                                <td>03-12</td>
                                <td>John Hall - (403) 861-7568</td>
                                <td>35-81,35-67,29-67</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            ";

            return content;
        }

        private string GetSingerPilots()
        {
            string content = @"
                <div id=""other_equipment"" class=""section"">
                    <span class=""heading"">Pilot Car and Other Equipment (Singer Service)</span>
                    
                    <table>
                        <thead>
                            <tr>
                                <th>Unit</th>
                                <th>Contact</th>                        
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>01-47</td>
                                <td>Cody LaFrance - (403) 796-3636</td>                        
                            </tr>
                            <tr>
                                <td>01-55</td>
                                <td>Jordy Cropley - (403) 816-1645 - Supervisor</td>
                            </tr>
                            <tr>
                                <td>99-09</td>
                                <td>Wyatt Singer - (403) 816-1640</td>
                            </tr>
                        </tbody>
                   </table>
                </div>
            ";

            return content;
        }

        private string GetThirdPartyPilots()
        {
            string content = @"
                <div id=""third_party_pilot"" class=""section"">
                    <span class=""heading"">Pilot Car (Thrid Party)</span>
                </div>
            ";

            return content;
        }

        private string GetThridPartyServices()
        {
            string content = @"
                <div id=""thid_party_services"" class=""section"">
                    <span class=""heading"">Third Party Services</span>
                    
                    <table>
                        <thead>
                            <tr>
                                <th>Date &amp Time</th>
                                <th>Service Type</th>
                                <th>Company</th>
                                <th>Contact</th>
                                <th>Location</th>
                                <th>Phone</th>
                                <th>Confirmation #</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>Jan 7, 2009 09:00</td>
                                <td>Light Swinging</td>
                                <td>City of Airdrie</td>
                                <td>Gayle</td>
                                <td>404 East Lake Rd, Airdrie</td>
                                <td>(403) 948-8415</td>
                                <td>Booked</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""7"">
                                    <span class=""field_name"">Comments:</span> Booked by Gayle for escort and light swinging
                                </td>
                            </tr>
                        </tbody>                
                    </table>
                </div>
            ";

            return content;
        }

        private string GetWireLiftInfo()
        {
            string content = @"
                <div id=""wire_lifts"" class=""section"">
                    <span class=""heading"">Wire Lift Information</span>
                    
                    <table>
                        <thead>
                            <tr>
                                <th>Date &amp; Time</th>
                                <th>Company</th>
                                <th>Contact</th>
                                <th>Location</th>
                                <th>Phone</th>
                                <th>Confirmation #</th>                        
                            </tr>                    
                        </thead>
                        <tbody>
                            <tr>
                                <td>Jan 7, 2009 08:00</td>
                                <td>Fortis Alberta</td>
                                <td>24 Hour Dispatch</td>
                                <td>404 East Lake Rd, Airdrie</td>
                                <td>(888) 251-3907</td>
                                <td>60066597</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""6"">
                                    <span class=""field_name"">Comments: </span> Fortis to meet, measure, and escort in Airdrie, and then escort in Sundre area
                                </td>
                            </tr>
                            <tr>
                                <td>Jan 7, 2009 9:51</td>
                                <td>Telus Communications</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""6"">
                                    <span class=""field_name"">Comments: </span> Cleared by Trevor
                                </td>
                            </tr>
                            <tr>
                                <td>Jan 7, 2009 09:52</td>
                                <td>Shaw Cable</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""6"">
                                    <span class=""field_name"">Comments: </span> Cleared by Earl
                                </td>
                            </tr>
                            <tr>
                                <td>Jan 7, 2009</td>
                                <td>Central Alberta REA</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""6"">
                                    <span class=""field_name"">Comments: </span> Cleared by CAREA
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            ";

            return content;
        }

        private string GetPermits()
        {
            string content = @"
                <div id=""permits"" class=""section"">
                    <span class=""heading"">Permit Information</span>
                    
                    <table>
                        <thead>
                            <tr>
                                <th>Date</th>
                                <th>Issuer</th>
                                <th>Type</th>
                                <th>Number</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>Jan 7, 2009 to Jan 13, 2009</td>
                                <td>Alberta Transportation</td>
                                <td>Empty Trailer - Overweight and Overdimensional</td>
                                <td>08-162-4298 - To Aidrie</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""4"">
                                    <span class=""field_name"">Conditions:</span> See permit
                                </td>
                            </tr>
                            <tr>
                                <td>Jan 7, 2009 to Jan 13, 2009</td>
                                <td>Alberta Transport</td>
                                <td>Overweight and Overdimensional</td>
                                <td>08-162-8380 - Loaded</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""4"">
                                    <span class=""field_name"">Conditions:</span> See permit
                                </td>
                            </tr>
                            <tr>
                                <td>Jan 7, 2009 to Jan 13, 2009</td>
                                <td>Alberta Transportation</td>
                                <td>Empty Trailer - Overweight and Overdimensional</td>
                                <td>08-162-4397 - Return</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""4"">
                                    <span class=""field_name"">Conditions:</span> See permit
                                </td>
                            </tr>
                            <tr>
                                <td>Jan 7, 2009 to Jan 13, 2009</td>
                                <td>County 17 Mountainview</td>
                                <td>Overweight and Overdimensional</td>
                                <td>PENDING</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""4"">
                                    <span class=""field_name"">Conditions:</span> For use of Twp320 and RR402
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            ";

            return content;
        }

        private string GetOtherInfo()
        {
            string content = @"
                <div id=""other_info"" class=""section"">
                    <span class=""heading"">Other Information</span>
                </div>
            ";

            return content;
        }
    }
}
