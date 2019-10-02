using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizableSkinController : MonoBehaviour 
{
	public Material material;

	public Slider[] colorA;
	public Slider[] colorB;
	public Slider[] colorC;

	public Slider shirtsSlider;
	public Slider shortsSlider;
	public Slider socksSlider;

	public Texture2D[] kits;

	Color _colorA = Color.black;
	Color _colorB = Color.black;
	Color _colorC = Color.black;

	void Awake()
	{
		colorA[0].onValueChanged.AddListener (delegate {OnColorAUpdate (0);});	
		colorA[1].onValueChanged.AddListener (delegate {OnColorAUpdate (1);});	
		colorA[2].onValueChanged.AddListener (delegate {OnColorAUpdate (2);});	
		colorB[0].onValueChanged.AddListener (delegate {OnColorBUpdate (0);});	
		colorB[1].onValueChanged.AddListener (delegate {OnColorBUpdate (1);});	
		colorB[2].onValueChanged.AddListener (delegate {OnColorBUpdate (2);});	
		colorC[0].onValueChanged.AddListener (delegate {OnColorCUpdate (0);});	
		colorC[1].onValueChanged.AddListener (delegate {OnColorCUpdate (1);});	
		colorC[2].onValueChanged.AddListener (delegate {OnColorCUpdate (2);});	

		shirtsSlider.onValueChanged.AddListener (delegate {OnShirtUpdate ();});	
		shortsSlider.onValueChanged.AddListener (delegate {OnShortsUpdate ();});	
		socksSlider.onValueChanged.AddListener (delegate {OnSocksUpdate ();});	
	}

	void OnShirtUpdate()
	{
		//TODO: Linkear Textura de Camiseta con Shader
		material.SetTexture("_Shirt", kits[(int)shirtsSlider.value]);
	}

	void OnShortsUpdate()
	{
		//TODO: Linkear Textura de Short con Shader
		material.SetTexture("_Shorts", kits[(int)shortsSlider.value]);
	}

	void OnSocksUpdate()
	{
		//TODO: Linkear Textura de Medias con Shader
		material.SetTexture("_Socks", kits[(int)socksSlider.value]);
	}

	public void OnColorAUpdate(int index)
	{
		switch (index) 
		{
		case 0: 
			_colorA.r = colorA [index].value;			
			break;
		case 1: 
			_colorA.g = colorA [index].value;			
			break;
		case 2: 
			_colorA.b = colorA [index].value;			
			break;
		default:
			break;
		}

		//TODO: Linkear Color con Shader
		material.SetColor("_ColorA", _colorA);
	}

	public void OnColorBUpdate(int index)
	{
		switch (index) 
		{
		case 0: 
			_colorB.r = colorB [index].value;			
			break;
		case 1: 
			_colorB.g = colorB [index].value;			
			break;
		case 2: 
			_colorB.b = colorB [index].value;			
			break;
		default:
			break;
		}

		//TODO: Linkear Color con Shader
		material.SetColor("_ColorB", _colorB);
	}

	public void OnColorCUpdate(int index)
	{
		switch (index) 
		{
		case 0: 
			_colorC.r = colorC [index].value;			
			break;
		case 1: 
			_colorC.g = colorC [index].value;			
			break;
		case 2: 
			_colorC.b = colorC [index].value;			
			break;
		default:
			break;
		}

		//TODO: Linkear Color con Shader
		material.SetColor("_ColorC", _colorC);
	}
}
