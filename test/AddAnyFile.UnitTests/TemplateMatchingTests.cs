namespace AddAnyFile.UnitTests
{
	using FluentAssertions;

	using MadsKristensen.AddAnyFile;

	using System.Collections.Generic;
	using System.Linq;

	using Xunit;

    public class TemplateMatchingTests
    {
		private const string _templateFolder = @"C:\User\AppData\Microsoft\VisualStudio\Extensions\AddAnyFile\.templates\";
		private const string _defaultExt = ".txt";
		private readonly IEnumerable<string> _templateFiles = new List<string>
		{
			".bowerrc.txt",
			".cs-interface.txt",
			".cs.txt",
			".html.txt",
			".json.txt",
			".md.txt",
			".razor.txt",
			".vb-interface.txt",
			".vb.txt",
			".bower.json.txt",
			".gruntfile.js.txt",
			".gulpfile.js.txt",
			".package.json.txt",
		}.Select(fileName => _templateFolder + fileName);

		[Trait("TemplateMatching", "GetMatchingTemplateFromFileName")]
        [Theory]
		[InlineData(".bowerrc")]
		[InlineData(".bower.json")]
		[InlineData(".gruntfile.js")]
		[InlineData(".gulpfile.js")]
		[InlineData(".package.json")]
        public void It_should_return_the_default_template_with_its_name_being_the_exact_name_of_the_new_file(string fileName)
        {
			// ARRANGE
			var templateFiles = _templateFiles.ToList();

			// ACT
			var result = TemplateMatching.GetMatchingTemplateFromFileName(templateFiles, fileName, _defaultExt);

			// ASSERT
			result.Should().Be(_templateFolder + fileName + _defaultExt);
        }

		[Trait("TemplateMatching", "GetMatchingTemplateFromFileName")]
        [Theory]
		[InlineData("Dockerfile", "Dockerfile.txt")]
		[InlineData(".gitignore", ".gitignore.txt")]
		[InlineData(".vimrc", ".vimrc.txt")]
        public void It_should_return_the_custom_template_with_its_name_being_the_exact_name_of_the_new_file(string fileName, string templateFileName)
        {
			// ARRANGE
			var templateFiles = _templateFiles.Prepend(_templateFolder + templateFileName).ToList();

			// ACT
			var result = TemplateMatching.GetMatchingTemplateFromFileName(templateFiles, fileName, _defaultExt);

			// ASSERT
			result.Should().Be(_templateFolder + templateFileName);
        }

		[Trait("TemplateMatching", "GetMatchingTemplateFromFileName")]
        [Theory]
		[InlineData("WeatherController.cs", "controller.txt")]
		[InlineData("UserService.cs", "service.txt")]
		[InlineData("UserLocationSubscriber.cs", "subscriber.txt")]
        public void It_should_return_the_template_with_its_name_being_the_end_of_the_new_file_name(string fileName, string templateFileName)
        {
			// ARRANGE
			var templateFiles = _templateFiles.Prepend(_templateFolder + templateFileName).ToList();

			// ACT
			var result = TemplateMatching.GetMatchingTemplateFromFileName(templateFiles, fileName, _defaultExt);

			// ASSERT
			result.Should().Be(_templateFolder + templateFileName);
        }

		[Trait("TemplateMatching", "GetMatchingTemplateFromFileName")]
        [Theory]
		[InlineData("WeatherController.cs", ".cs.txt")]
		[InlineData("user.component.html", ".html.txt")]
		[InlineData("config.json", ".json.txt")]
		[InlineData("README.md", ".md.txt")]
		[InlineData("UserComponent.razor", ".razor.txt")]
		[InlineData("UserService.vb", ".vb.txt")]
        public void It_should_return_the_template_with_its_name_being_the_extension_of_the_file_name(string fileName, string templateFileName)
        {
			// ARRANGE
			var templateFiles = _templateFiles.ToList();

			// ACT
			var result = TemplateMatching.GetMatchingTemplateFromFileName(templateFiles, fileName, _defaultExt, filePath => true);

			// ASSERT
			result.Should().Be(_templateFolder + templateFileName);
        }
    }
}