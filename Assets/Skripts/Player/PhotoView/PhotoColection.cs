using System.Collections.Generic;
using UnityEngine;

public class PhotoColection {
    List<PhotoPictures> _photoColection;
    int curentIndex = 0;
    public PhotoColection()
    {
        _photoColection = new();
    }
    public void NextPhoto()
    {
        _photoColection[curentIndex].ClousedPhoto();

        curentIndex++;
        if(curentIndex == _photoColection.Count) curentIndex = 0;
        _photoColection[curentIndex].OpenFhoto();
    }
    public void PrewPhoto()
    {
        _photoColection[curentIndex].ClousedPhoto();
        
        curentIndex--;
        if(curentIndex < 0) curentIndex = _photoColection.Count - 1;
        _photoColection[curentIndex].OpenFhoto();
    }
    
    public void OpenPhoto() => _photoColection[curentIndex].OpenFhoto();
    public void ClousedPhoto() => _photoColection[curentIndex].ClousedPhoto();
    
    public void AddPhoto(PhotoPictures newPhoto)
    {
        if(!_photoColection.Contains(newPhoto))
        {
            curentIndex = PhotoCount();
            _photoColection.Add(newPhoto); 
        }

    }
    public int PhotoCount() => _photoColection.Count;
    
}