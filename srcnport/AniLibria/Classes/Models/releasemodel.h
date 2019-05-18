#ifndef RELEASEMODEL_H
#define RELEASEMODEL_H

#include <QtCore>

class ReleaseModel
{
private:
    int m_Id;
    QString m_Code;
    QString m_Series;
    QString m_Poster;
    QString m_Timestamp;
    QString m_Status;
    QString m_Type;
    QString m_Year;
    QString m_Description;
    QStringList m_Genres;
    QStringList m_Voices;
    QStringList m_Names;
    int m_Rating;
    bool m_IsBlocked;

public:
    ReleaseModel();

    void readFromApiModel(const QJsonObject &jsonObject);


};

#endif // RELEASEMODEL_H
