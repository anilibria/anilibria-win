#ifndef RELEASEVIDEOMODEL_H
#define RELEASEVIDEOMODEL_H

#include <QObject>

class ReleaseVideoModel : public QObject
{
    Q_OBJECT

    Q_PROPERTY(int id READ id WRITE setId NOTIFY idChanged)
    Q_PROPERTY(QString title READ title WRITE setTitle NOTIFY titleChanged)
    Q_PROPERTY(QString sd READ sd WRITE setSd NOTIFY sdChanged)
    Q_PROPERTY(QString hd READ hd WRITE setHd NOTIFY hdChanged)
    Q_PROPERTY(QString fullhd READ fullhd WRITE setFullhd NOTIFY fullhdChanged)
    Q_PROPERTY(QString srcsd READ srcsd WRITE setSrcsd NOTIFY srcsdChanged)
    Q_PROPERTY(QString srchd READ srchd WRITE setSrchd NOTIFY srchdChanged)

public:
    explicit ReleaseVideoModel(QObject *parent = nullptr);

    int id() const;
    void setId(const int id);

    QString title() const;
    void setTitle(const QString &title);

    QString sd() const;
    void setSd(const QString &sd);

    QString hd() const;
    void setHd(const QString &hd);

    QString fullhd() const;
    void setFullhd(const QString &fullhd);

    QString srcsd() const;
    void setSrcsd(const QString &srcsd);

    QString srchd() const;
    void setSrchd(const QString &srchd);

private:
    int m_Id;
    QString m_Title;
    QString m_Sd;
    QString m_Hd;
    QString m_FullHd;
    QString m_SrcSd;
    QString m_SrcHd;

signals:
    void titleChanged();
    void sdChanged();
    void hdChanged();
    void fullhdChanged();
    void srcsdChanged();
    void srchdChanged();
    void idChanged();

public slots:
};

#endif // RELEASEVIDEOMODEL_H
