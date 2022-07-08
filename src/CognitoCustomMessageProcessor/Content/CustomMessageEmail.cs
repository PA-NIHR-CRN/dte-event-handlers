namespace CognitoCustomMessageProcessor.Content
{
    public static class CustomMessageEmail
    {
        // TODO - html file or S3, make the text replaceable so can replace for difference trigger sources.
        public static string GetCustomMessageHtml()
        {
            return @"<!DOCTYPE html>
<html
  xmlns=""http://www.w3.org/1999/xhtml""
  xmlns:v=""urn:schemas-microsoft-com:vml""
  xmlns:o=""urn:schemas-microsoft-com:office:office""
>
  <head>
    <title></title>
    <!--[if !mso]><!-- -->
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
    <!--<![endif]-->
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />

    <!--[if !mso]><!-->

    <!--<![endif]-->
    <!--[if mso
      ]><xml>
        <o:OfficeDocumentSettings>
          <o:AllowPNG /> <o:PixelsPerInch>96</o:PixelsPerInch>
        </o:OfficeDocumentSettings></xml
      ><!
    [endif]-->
    <!--[if lte mso 11
      ]><style type=""text/css"">
        .outlook-group-fix {
          width: 100% !important;
        }
      </style><!
    [endif]-->
    <!--[if !mso]><!-->

    <!--<![endif]-->
  </head>
  <body
    style=""
      background-color: #f5f5f5;
      margin: 0;
      padding: 0;
      -webkit-text-size-adjust: 100%;
      -ms-text-size-adjust: 100%;
    ""
  >
    <div class=""mj-container"">
      <!--[if mso | IE]>      <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center"" style=""width:600px;"">        <tr>          <td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;"">      <![endif]-->
      <div
        style=""margin: 18px auto 0 auto; max-width: 600px; background: #ffffff""
      >
        <table
          role=""presentation""
          style=""
            font-size: 0px;
            width: 100%;
            background: #ffffff;
            border-collapse: collapse;
            mso-table-lspace: 0pt;
            mso-table-rspace: 0pt;
            padding: 0 10px;
          ""
          align=""center""
          border=""0""
        >
          <tbody>
            <tr>
              <td
                style=""
                  text-align: center;
                  vertical-align: middle;
                  direction: ltr;
                  font-size: 0px;
                  border-collapse: collapse;
                  mso-table-lspace: 0pt;
                  mso-table-rspace: 0pt;
                ""
              >
                <!--[if mso | IE]>      <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"">        <tr>          <td style=""vertical-align:middle;width:600px;"">      <![endif]-->
                <div
                  class=""mj-column-per-100 outlook-group-fix""
                  style=""
                    vertical-align: middle;
                    display: inline-block;
                    direction: ltr;
                    font-size: 13px;
                    text-align: left;
                    width: 100%;
                  ""
                >
                  <table
                    role=""presentation""
                    cellpadding=""0""
                    cellspacing=""0""
                    style=""
                      vertical-align: middle;
                      border-collapse: collapse;
                      mso-table-lspace: 0pt;
                      mso-table-rspace: 0pt;
                      padding: 0 10px;
                    ""
                    width=""100%""
                    border=""0""
                  >
                    <tbody>
                      <tr>
                        <td
                          style=""
                            word-wrap: break-word;
                            font-size: 0px;
                            border-collapse: collapse;
                            mso-table-lspace: 0pt;
                            mso-table-rspace: 0pt;
                          ""
                        >
                          <div
                            style=""
                              font-size: 1px;
                              line-height: 10px;
                              white-space: nowrap;
                            ""
                          >
                            &#xA0;
                          </div>
                        </td>
                      </tr>
                      <tr>
                        <td
                          style=""
                            word-wrap: break-word;
                            font-size: 0px;
                            padding: 0px 0px 0px 0px;
                            border-collapse: collapse;
                            mso-table-lspace: 0pt;
                            mso-table-rspace: 0pt;
                          ""
                        >
                          <table
                            role=""presentation""
                            cellpadding=""0""
                            cellspacing=""0""
                            style=""
                              border-collapse: collapse;
                              border-spacing: 0px;
                              width: 100%;
                              mso-table-lspace: 0pt;
                              mso-table-rspace: 0pt;
                              padding: 0 10px;
                            ""
                            border=""0""
                          >
                            <tbody>
                              <tr>
                                <td
                                  style=""
                                    width: 50%;
                                    border-collapse: collapse;
                                    mso-table-lspace: 0pt;
                                    mso-table-rspace: 0pt;
                                  ""
                                  align=""left""
                                >
                                  <img
                                    alt=""Be Part of Research Logo""
                                    src=""https://bepartofresearch.nihr.ac.uk/dA/67fb23bffd/BPoR%20colour%20RGB.jpg""
                                    style=""
                                      width: 150px;
                                      height: auto;
                                      margin: 0 18px;
                                      border: 0;
                                      line-height: 100%;
                                      outline: none;
                                      text-decoration: none;
                                      -ms-interpolation-mode: bicubic;
                                    ""
                                  />
                                </td>
                                <td
                                  style=""
                                    width: 50%;
                                    border-collapse: collapse;
                                    mso-table-lspace: 0pt;
                                    mso-table-rspace: 0pt;
                                  ""
                                  align=""right""
                                >
                                  <img
                                    alt
                                    src=""https://bepartofresearch.nihr.ac.uk/dA/84c0a77dd7/NHS.png""
                                    style=""
                                      width: 100px;
                                      height: auto;
                                      margin: 0 18px;
                                      border: 0;
                                      line-height: 100%;
                                      outline: none;
                                      text-decoration: none;
                                      -ms-interpolation-mode: bicubic;
                                    ""
                                  />
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                <!--[if mso | IE]>      </td></tr></table>      <![endif]-->
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <!--[if mso | IE]>      </td></tr></table>      <![endif]-->
      <!--[if mso | IE]>      <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center"" style=""width:600px;"">        <tr>          <td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;"">      <![endif]-->
      <div style=""margin: 0px auto; max-width: 600px; background: #ffffff"">
        <table
          role=""presentation""
          cellpadding=""0""
          cellspacing=""0""
          style=""
            font-size: 0px;
            width: 100%;
            background: #ffffff;
            border-collapse: collapse;
            mso-table-lspace: 0pt;
            mso-table-rspace: 0pt;
            padding: 0 10px;
          ""
          align=""center""
          border=""0""
        >
          <tbody>
            <tr>
              <td
                style=""
                  text-align: center;
                  vertical-align: top;
                  direction: ltr;
                  font-size: 0px;
                  padding: 0px 0px 0px 0px;
                  border-collapse: collapse;
                  mso-table-lspace: 0pt;
                  mso-table-rspace: 0pt;
                ""
              >
                <!--[if mso | IE]>      <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"">        <tr>          <td style=""vertical-align:top;width:600px;"">      <![endif]-->
                <div
                  class=""mj-column-per-100 outlook-group-fix""
                  style=""
                    vertical-align: top;
                    display: inline-block;
                    direction: ltr;
                    font-size: 13px;
                    text-align: left;
                    width: 100%;
                  ""
                >
                  <table
                    role=""presentation""
                    cellpadding=""0""
                    cellspacing=""0""
                    style=""
                      vertical-align: top;
                      border-collapse: collapse;
                      mso-table-lspace: 0pt;
                      mso-table-rspace: 0pt;
                      padding: 0 10px;
                    ""
                    width=""100%""
                    border=""0""
                  >
                    <tbody>
                      <tr>
                        <td
                          style=""
                            word-wrap: break-word;
                            font-size: 0px;
                            border-collapse: collapse;
                            mso-table-lspace: 0pt;
                            mso-table-rspace: 0pt;
                          ""
                        >
                          <div
                            style=""
                              font-size: 1px;
                              line-height: 15px;
                              white-space: nowrap;
                            ""
                          >
                            &#xA0;
                          </div>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                <!--[if mso | IE]>      </td></tr></table>      <![endif]-->
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <!--[if mso | IE]>      </td></tr></table>      <![endif]-->
      <!--[if mso | IE]>      <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center"" style=""width:600px;"">        <tr>          <td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;"">      <![endif]-->
      <div style=""margin: 0px auto; max-width: 600px; background: #ffffff"">
        <table
          role=""presentation""
          cellpadding=""0""
          cellspacing=""0""
          style=""
            font-size: 0px;
            width: 100%;
            background: #ffffff;
            margin: 0 1px;
            border-collapse: collapse;
            mso-table-lspace: 0pt;
            mso-table-rspace: 0pt;
            padding: 0 10px;
          ""
          align=""center""
          border=""0""
        >
          <tbody>
            <tr>
              <td
                style=""
                  text-align: center;
                  vertical-align: top;
                  direction: ltr;
                  font-size: 0px;
                  padding: 9px 0px 9px 0px;
                  border-collapse: collapse;
                  mso-table-lspace: 0pt;
                  mso-table-rspace: 0pt;
                ""
              >
                <!--[if mso | IE]>      <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"">        <tr>          <td style=""vertical-align:top;width:600px;"">      <![endif]-->
                <div
                  class=""mj-column-per-100 outlook-group-fix""
                  style=""
                    vertical-align: top;
                    display: inline-block;
                    direction: ltr;
                    font-size: 13px;
                    text-align: left;
                    width: 100%;
                  ""
                >
                  <table
                    role=""presentation""
                    cellpadding=""0""
                    cellspacing=""0""
                    style=""
                      vertical-align: top;
                      border-collapse: collapse;
                      mso-table-lspace: 0pt;
                      mso-table-rspace: 0pt;
                      padding: 0 10px;
                    ""
                    width=""100%""
                    border=""0""
                  >
                    <tbody>
                      <tr>
                        <td
                          style=""
                            word-wrap: break-word;
                            font-size: 0px;
                            padding: 0px 0px 0px 0px;
                            border-collapse: collapse;
                            mso-table-lspace: 0pt;
                            mso-table-rspace: 0pt;
                          ""
                        >
                          <div
                            style=""
                              cursor: auto;
                              color: #000000;
                              font-family: Lato, Helvetica, Arial, sans-serif;
                              font-size: 11px;
                              line-height: 1.5;
                              margin: 0 18px;
                            ""
                          >
                            <p style=""display: block; margin: 13px 0"">
                              <span style=""font-size: 24px; color: #193e72""
                                ><strong
                                  >###TITLE_REPLACE1###
                                </strong></span
                              >
                            </p>
                            <p style=""display: block; margin: 13px 0"">
                              <span style=""font-size: 16px""
                                >###TEXT_REPLACE1###
                              </span>
                            </p>
                            <p style=""display: block; margin: 13px 0"">
                              <span style=""font-size: 16px""
                                >###TEXT_REPLACE2###
                              </span>
                            </p>
                            <p style=""display: ###LINK_DISPLAY_VALUE_REPLACE###; margin: 13px 0"">
                              <span style=""font-size: 16px""
                                >###LINK_REPLACE###
                              </span>
                            </p>
                            <p style=""display: block; margin: 13px 0"">
                              <span style=""font-size: 16px""
                                >###TEXT_REPLACE3###
                              </span>
                            </p>
                            <p style=""display: block; margin: 13px 0"">
                              <span style=""font-size: 16px""
                                >The Be Part of Research team
                              </span>
                            </p>
                            <hr />
                            <p style=""display: block; margin: 13px 0"">
                              <span style=""font-size: 16px""
                                >This is an automated email, please do not reply
                                directly to this email address.
                              </span>
                            </p>
                            <p style=""display: block; margin: 13px 0"">
                              <span style=""font-size: 16px""
                                >If you have any questions about your Be Part of
                                Research account or taking part in research, see
                                our
                                <a
                                  href=""https://bepartofresearch.nihr.ac.uk/volunteer-service/""
                                  target=""_blank""
                                  rel=""noopener noreferrer""
                                >
                                  frequently asked questions.
                                </a></span
                              >
                            </p>
                          </div>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                <!--[if mso | IE]>      </td></tr></table>      <![endif]-->
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <!--[if mso | IE]>      </td></tr></table>      <![endif]-->
    </div>
  </body>
</html>";
        }
    }
}
