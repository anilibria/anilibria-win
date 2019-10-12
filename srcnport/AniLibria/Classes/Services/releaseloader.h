#ifndef RELEASELOADER_H
#define RELEASELOADER_H

#include <QObject>
#include "../Models/releaseitemmodel.h"

class ReleaseLoader : public QObject
{
    Q_OBJECT

private:
    QList<ReleaseModel>* list = new QList<ReleaseModel>();
public:
    explicit ReleaseLoader(QObject *parent = nullptr);

    QList<ReleaseModel> releases() const;

signals:

    void loadedReleases();

public slots:
    void loadReleases();
};

#endif // RELEASELOADER_H
