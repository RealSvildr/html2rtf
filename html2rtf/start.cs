using System;
using System.Collections.Generic;

namespace html2rtf
{
    public class Start
    {
        public static void Main() {
            var html = @"
				<style>
					.body { font-size: 12.5px; font-family: arial; }
					.title { font-size: 36px; text-align: center; }
					.sep { margin: 26px 0 0; font-size: 16px; font-weight: bold; }
					.right { text-align: right; margin: -17px 0; }
					.nome_empresa { font-weight: bold; text-decoration: underline; }
					.border_none, .border_none tr, .border_none td { border: none; margin: 0; }
					.info { margin-bottom: 15px; }
			
					hr { margin: 2px 0 8px 0; border: none; height: 2px; background: #666; }
					.exp {  height: 0; border: dashed 1px #666c; background: none; margin: 8px 0 3px; }
				</style>
				<span class='body' style='margin: 5px'>
					<div class='title'>Candidato</div>
				</span>
				<div class='sep'>DADOS PESSOAIS</div><hr>
				<div class='info'>
					Data de Nascimento: 00/00/0000<br>
					Endereço: Endereço Magico nº 7, complemento – Bairro  – Cidade</br>
				</div>

			";
			var htmlHeader = @"";
			var htmlFooter = @"";

			//paper-width
			//paper-height
			//margin-top
			//margin-bottom
			//margin-left
			//margin-right

			//font-size
			//font-family

			//header-font-size
			//header-distance-edge
			//footer-font-size
			//footer-distance-edge
			var extraParams = new Dictionary<string, string>();
			

			var html_parser = new HtmlParser();
			var rtf_parser = new RtfParser();
			var page = html_parser.Parse(htmlHeader, html, htmlFooter);

			rtf_parser.Parse(page, extraParams);

        }
    }
}
